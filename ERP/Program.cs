using ERP.Application.Asset.Information.Services;
using ERP.Application.Asset.Model.Interfaces;
using ERP.Application.Auth.Interfaces;
using ERP.Application.Auth.Interfaces.Security;
using ERP.Application.Common;
using ERP.Application.Department.Interfaces;
using ERP.Application.StorageLocation.Interfaces;
using ERP.Application.Systems.Interfaces;
using ERP.Infrastructure.Auth;
using ERP.Infrastructure.ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                  "http://localhost:5173",
                  "https://localhost:5173",
                  "https://192.168.1.19:5173"
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// ✅ Gộp 1 lần: Global Authorize + JsonOptions
builder.Services.AddControllers()
.AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAssetBigTypeService, AssetBigTypeService>();
builder.Services.AddScoped<IAssetMiddleTypeService, AssetMiddleTypeService>();
builder.Services.AddScoped<IAssetSmallTypeService, AssetSmallTypeService>();
builder.Services.AddScoped<IAssetModelService, AssetModelService>();
builder.Services.AddScoped<IAssetInformationService, AssetInformationService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IStorageLocationService, StorageLocationService>();
builder.Services.AddScoped<ISystemService, SystemService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<ITranslationService, TranslationService>();
builder.Services.AddScoped<IMailService, SmtpMailService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IAssetHandoverService, AssetHandoverService>();
builder.Services.AddScoped<IAssetScrapService, AssetScrapService>();
builder.Services.AddScoped<IAssetRepairService, AssetRepairService>();
builder.Services.AddScoped<IAssetReportService, AssetReportService>();
builder.Services.AddScoped<IAssetLiquidationService, AssetLiquidationService>();
builder.Services.AddScoped<ISequenceGenerator, SequenceGenerator>();
builder.Services.AddHostedService<HandoverExpiryService>();
builder.Services.AddScoped<JwtTokenService>();

//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContextPool<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sql =>
        {
            sql.EnableRetryOnFailure(maxRetryCount: 3);
            sql.MaxBatchSize(200); // gộp tối đa 200 lệnh/round-trip thay vì 1-1
        }));

// ✅ Cache trong RAM (process-local) cho dữ liệu danh mục ít thay đổi
// (Departments, Big/Middle/Small Types...) → giảm số lượt query thẳng vào SQL Server
// khi có nhiều request đồng thời. Đủ dùng cho mô hình chạy 1 instance hiện tại.
builder.Services.AddMemoryCache();

// ✅ Output Cache: cache response HTTP cho các API GET dữ liệu danh mục ít thay đổi
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("MasterData", policy =>
        policy.Expire(TimeSpan.FromMinutes(10))
              .SetVaryByQuery("*"));
});

// ✅ Rate Limiting: chặn spam request từ 1 client, bảo vệ DB khỏi quá tải
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("ApiLimiter", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;           // tối đa 100 request
        limiterOptions.Window = TimeSpan.FromSeconds(10); // mỗi 10 giây / IP
        limiterOptions.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 20;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();

// ✅ Đúng thứ tự: Routing → CORS → Auth → Authorization → Endpoints
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();   // ✅ chặn spam request trước khi vào controller
app.UseOutputCache();   // ✅ áp dụng cache cho các endpoint có [OutputCache]

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
// 1. Cấu hình định nghĩa định dạng file (MIME Types) cho file 3D
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".glb"] = "model/gltf-binary";
provider.Mappings[".gltf"] = "model/gltf+json";

// 2. Phục vụ file tĩnh từ thư mục "public" kèm cấu hình MIME Type
app.UseStaticFiles();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
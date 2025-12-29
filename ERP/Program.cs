using ERP.Application.Auth.Interfaces;
using ERP.Application.Auth.Interfaces.Security;
using ERP.Application.Systems.Interfaces;
using ERP.Infrastructure.Auth;
using ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.Services;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ISystemService, SystemService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITranslationService, TranslationService>();
builder.Services.AddScoped<IMailService,SmtpMailService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<JwtTokenService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

if (!app.Environment.IsDevelopment())
{
    app.UseSpaStaticFiles();
}
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();

using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Asset.Model.Interfaces;
using ERP.Application.Common;
using ERP.Infrastructure.ERP.Domain.Entities;
using ERP.Infrastructure.ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ERP.Infrastructure.Services
{
    public class AssetModelService : IAssetModelService
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Các định dạng file model 3D được phép upload cho CAD/3D Visualization.
        /// </summary>
        private static readonly string[] AllowedModel3DExtensions = { ".glb", ".gltf" };

        public AssetModelService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<AssetModelListDto>> SearchAsync(
            AssetModelSearchDto dto)
        {
            var query =
                from m in _context.Asset_Models.AsNoTracking()
                join b in _context.Asset_Big_Types on m.Big_Type_Id equals b.Big_Type_Id
                join mid in _context.Asset_Middle_Types on m.Middle_Type_Id equals mid.Middle_Type_Id
                join s in _context.Asset_Small_Types on m.Small_Type_Id equals s.Small_Type_Id
                join u1 in _context.Users on m.CreatedBy equals u1.Id
                join u2 in _context.Users on m.UpdatedBy equals u2.Id into uu
                from u2 in uu.DefaultIfEmpty()
                select new { m, b, mid, s, u1, u2 };

            /* ================= FILTER ================= */

            if (dto.BigTypeId.HasValue)
                query = query.Where(x => x.b.Big_Type_Id == dto.BigTypeId);

            if (dto.MiddleTypeId.HasValue)
                query = query.Where(x => x.mid.Middle_Type_Id == dto.MiddleTypeId);

            if (dto.SmallTypeId.HasValue)
                query = query.Where(x => x.s.Small_Type_Id == dto.SmallTypeId);

            if (!string.IsNullOrWhiteSpace(dto.ModelName))
                query = query.Where(x =>
                    x.m.Model_Name.Contains(dto.ModelName));

            /* ================= COUNT ================= */

            var total = await query.CountAsync();

            /* ================= DATA ================= */

            var items = await query
                .OrderByDescending(x => x.m.CreatedAt)
                .Skip(dto.Skip)
                .Take(dto.Take)
                .Select(x => new AssetModelListDto
                {
                    ModelId = x.m.Model_Id,

                    BigTypeId = x.b.Big_Type_Id,
                    MiddleTypeId = x.m.Middle_Type_Id,
                    SmallTypeId = x.s.Small_Type_Id,

                    BigTypeNameVN = x.b.Big_Type_Name_VN,
                    MiddleTypeNameVN = x.mid.Middle_Type_Name_VN,
                    SmallTypeNameVN = x.s.Small_Type_Name_VN,

                    AssetFullName =
                        x.b.Big_Type_Name_VN + " - " +
                        x.mid.Middle_Type_Name_VN + " - " +
                        x.s.Small_Type_Name_VN,

                    ModelName = x.m.Model_Name,

                    ImageData = x.m.ImageData,

                    Model3DUrl = x.m.Model3DUrl,

                    MachineType =
                        x.mid.Machine_Type == "1"
                            ? "Máy móc"
                            : "Công dụng cụ",

                    InventoryCheckType =
                        x.mid.Inventory_Check_Type == "1"
                            ? "Kiểm số lượng"
                            : "Kiểm quét",

                    CreatedAt = x.m.CreatedAt,
                    CreatedBy = x.u1.UserName,

                    UpdatedAt = x.m.UpdatedAt,
                    UpdatedBy = x.u2 != null ? x.u2.UserName : null
                })
                .ToListAsync();

            return new PagedResult<AssetModelListDto>
            {
                Total = total,
                Items = items
            };

        }
        public async Task<List<AssetModelOptionDto>> GetOptionsAsync()
        {
            return await _context.Asset_Models
                .OrderBy(x => x.Model_Name)
                .Select(x => new AssetModelOptionDto
                {
                    Id = x.Model_Id,
                    Name = x.Model_Name
                })
                .ToListAsync();
        }

        public async Task<int> CreateAsync(
            CreateAssetModelDto dto,
            string username)
        {
            if (string.IsNullOrWhiteSpace(dto.ModelName))
                throw new ArgumentException("Model name không được để trống");

            var userId = await GetUserId(username);

            /* ===== FK CHECK ===== */

            await ValidateTypes(dto.BigTypeId, dto.MiddleTypeId, dto.SmallTypeId);

            var entity = new Asset_Models
            {
                Big_Type_Id = dto.BigTypeId,
                Middle_Type_Id = dto.MiddleTypeId,
                Small_Type_Id = dto.SmallTypeId,

                Model_Name = dto.ModelName.Trim(),

                ImageData = dto.ImageData ?? Array.Empty<byte>(),

                Model3DUrl = dto.Model3DUrl,

                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.Asset_Models.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Model_Id;
        }

        /* ================= UPDATE ================= */

        public async Task UpdateAsync(
            int id,
            UpdateAssetModelDto dto,
            string username)
        {
            var userId = await GetUserId(username);

            var entity = await _context.Asset_Models
                .FirstOrDefaultAsync(x => x.Model_Id == id);

            if (entity == null)
                throw new Exception("Model không tồn tại");

            entity.Model_Name = dto.ModelName.Trim();

            //if (dto.UpdateImage && dto.ImageData != null)
            if (dto.ImageData != null)
                entity.ImageData = dto.ImageData;

            if (dto.UpdateModel3D)
                entity.Model3DUrl = dto.Model3DUrl;

            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = userId;

            await _context.SaveChangesAsync();
        }

        /* ================= UPLOAD MODEL 3D ================= */

        public async Task<string> UploadModel3DAsync(string fileName, Stream fileStream)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (!AllowedModel3DExtensions.Contains(extension))
                throw new ArgumentException("Chỉ chấp nhận file .glb hoặc .gltf");

            // 1. ✅ Tìm thư mục gốc thực tế chứa folder "public"
            string rootPath = GetApplicationRoot();
            rootPath=rootPath.Substring(0, rootPath.Length - 3)+"erp.frontend";
            var folder = Path.Combine(rootPath, "public", "models", "asset-models-3d");

            Directory.CreateDirectory(folder);

            var safeFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(folder, safeFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(stream);
            }

            return $"/models/asset-models-3d/{safeFileName}";
        }
        private string GetApplicationRoot()
        {
            var currentDir = Directory.GetCurrentDirectory();

            // Nếu chạy debug từ IDE, đôi khi CurrentDirectory trỏ vào bin/Debug/..., cần nhảy ngược ra ngoài
            if (currentDir.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))
            {
                currentDir = currentDir.Split(new string[] { $"{Path.DirectorySeparatorChar}bin" }, StringSplitOptions.None)[0];
            }

            return currentDir;
        }

        /* ================= VALIDATE FK ================= */

        private async Task ValidateTypes(
            int bigTypeId,
            int middleTypeId,
            int smallTypeId)
        {
            if (!await _context.Asset_Big_Types
                .AnyAsync(x => x.Big_Type_Id == bigTypeId))
                throw new Exception("BigType không tồn tại");

            if (!await _context.Asset_Middle_Types
                .AnyAsync(x => x.Middle_Type_Id == middleTypeId))
                throw new Exception("MiddleType không tồn tại");

            if (!await _context.Asset_Small_Types
                .AnyAsync(x => x.Small_Type_Id == smallTypeId))
                throw new Exception("SmallType không tồn tại");
        }
        private async Task<Guid> GetUserId(string userName)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserName == userName);

            if (user == null)
                throw new Exception($"User [{userName}] không tồn tại");

            return user.Id;
        }
    }
}
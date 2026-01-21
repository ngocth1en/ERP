using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Asset.Model.Interfaces;
using ERP.Application.Common;
using ERP.Infrastructure.ERP.Domain.Entities;
using ERP.Infrastructure.ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Services
{
    public class AssetModelService : IAssetModelService
    {
        private readonly AppDbContext _context;

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

                    BigTypeNameVN = x.b.Big_Type_Name_VN,
                    MiddleTypeNameVN = x.mid.Middle_Type_Name_VN,
                    SmallTypeNameVN = x.s.Small_Type_Name_VN,

                    AssetFullName =
                        x.b.Big_Type_Name_VN + " - " +
                        x.mid.Middle_Type_Name_VN + " - " +
                        x.s.Small_Type_Name_VN,

                    ModelName = x.m.Model_Name,

                    ImageData = x.m.ImageData,

                    MachineType =
                        x.mid.Machine_Type == "1"
                            ? "Máy móc"
                            : "Công dụng cụ",

                    InventoryCheckType =
                        x.mid.Inventory_Check_Type == "1"
                            ? "Kiểm quét"
                            : "Kiểm số lượng",

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

            if (dto.UpdateImage && dto.ImageData != null)
                entity.ImageData = dto.ImageData;

            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = userId;

            await _context.SaveChangesAsync();
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

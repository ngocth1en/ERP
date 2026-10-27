using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Asset.Model.Interfaces;
using ERP.Application.Common;
using ERP.Infrastructure.ERP.Domain.Entities;
using ERP.Infrastructure.ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Services
{
    public class AssetSmallTypeService : IAssetSmallTypeService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private static string LookupCacheKey(int middleTypeId) => $"asset_small_types_lookup_{middleTypeId}";

        public AssetSmallTypeService(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        /* ============================
           SEARCH BY MIDDLE TYPE
        ============================ */
        public async Task<PagedResult<AssetSmallTypeListDto>> SearchAsync(
            AssetSmallTypeSearchDto dto)
        {
            var query = _context.Asset_Small_Types
                .AsNoTracking()
                .Where(x => x.Middle_Type_Id == dto.MiddleTypeId);

            if (!string.IsNullOrWhiteSpace(dto.Keyword))
            {
                query = query.Where(x =>
                    x.Small_Type_Name_EN.Contains(dto.Keyword) ||
                    x.Small_Type_Name_VN.Contains(dto.Keyword));
            }

            query = query.Where(x => x.IsActive == true);

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.Small_Type_Name_EN)
                .Skip(dto.Skip)
                .Take(dto.Take)
                .Select(x => new AssetSmallTypeListDto
                {
                    SmallTypeId = x.Small_Type_Id,
                    NameEN = x.Small_Type_Name_EN,
                    NameVN = x.Small_Type_Name_VN,
                    IsActive = true,
                    UpdatedAt = x.UpdatedAt
                })
                .ToListAsync();

            return new PagedResult<AssetSmallTypeListDto>
            {
                Total = total,
                Items = items
            };
        }

        /* ============================
           CREATE
        ============================ */
        public async Task<int> CreateAsync(
            CreateAssetSmallTypeDto dto,
            string username)
        {
            /* ========== VALIDATE ========== */

            if (dto.MiddleTypeId <= 0)
                throw new ArgumentException("MiddleTypeId không hợp lệ");

            if (string.IsNullOrWhiteSpace(dto.NameEN))
                throw new ArgumentException("Tên EN không được để trống");

            if (string.IsNullOrWhiteSpace(dto.NameVN))
                throw new ArgumentException("Tên VN không được để trống");

            /* ========== USER ========== */

            var userId = await GetUserId(username);

            /* ========== CHECK FK ========== */

            var existsMiddleType = await _context.Asset_Middle_Types
                .AnyAsync(x => x.Middle_Type_Id == dto.MiddleTypeId);

            if (!existsMiddleType)
                throw new InvalidOperationException(
                    $"MiddleTypeId = {dto.MiddleTypeId} không tồn tại");

            /* ========== CREATE ENTITY ========== */

            var entity = new Asset_Small_Types
            {
                Small_Type_Name_EN = dto.NameEN.Trim(),
                Small_Type_Name_VN = dto.NameVN.Trim(),

                Middle_Type_Id = dto.MiddleTypeId,
                IsActive = true,

                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.Asset_Small_Types.Add(entity);
            await _context.SaveChangesAsync();

            // ✅ Danh sách lookup theo MiddleTypeId này vừa thay đổi → xóa cache tương ứng
            _cache.Remove(LookupCacheKey(dto.MiddleTypeId));

            return entity.Small_Type_Id;
        }

        /* ============================
           UPDATE
        ============================ */
        public async Task UpdateAsync(
            int id,
            UpdateAssetSmallTypeDto dto,
            string username)
        {
            var userId = await GetUserId(username);

            var entity = await _context.Asset_Small_Types
                .FirstOrDefaultAsync(x => x.Small_Type_Id == id);

            if (entity == null)
                throw new Exception("Small Type not found");

            entity.Small_Type_Name_EN = dto.NameEN;
            entity.Small_Type_Name_VN = dto.NameVN;
            entity.IsActive = dto.IsActive;

            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            _cache.Remove(LookupCacheKey(entity.Middle_Type_Id));
        }

        /* ============================
           HIDE / SHOW
        ============================ */
        public async Task ToggleActiveAsync(int id, string username)
        {
            var userId = await GetUserId(username);

            var entity = await _context.Asset_Small_Types
                .FirstOrDefaultAsync(x => x.Small_Type_Id == id);

            if (entity == null)
                throw new Exception("Small Type not found");

            entity.IsActive = !entity.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            _cache.Remove(LookupCacheKey(entity.Middle_Type_Id));
        }

        /* ============================
           GET USER
        ============================ */
        private async Task<Guid> GetUserId(string userName)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserName == userName);

            if (user == null)
                throw new Exception($"User [{userName}] không tồn tại");

            return user.Id;
        }

        /// <summary>
        /// Lookup theo MiddleTypeId — dùng cho dropdown phụ thuộc (cascading select).
        /// ✅ Cache 15 phút theo từng middleTypeId để giảm tải DB khi nhiều user cùng mở form.
        /// </summary>
        public async Task<List<LookupDto>> GetByMiddleTypeAsync(int middleTypeId)
        {
            var cached = await _cache.GetOrCreateAsync(LookupCacheKey(middleTypeId), async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

                return await _context.Asset_Small_Types
                    .AsNoTracking()
                    .Where(x => x.Middle_Type_Id == middleTypeId && x.IsActive)
                    .OrderBy(x => x.Small_Type_Name_VN)
                    .Select(x => new LookupDto
                    {
                        Id = x.Small_Type_Id,
                        Name = x.Small_Type_Name_VN
                    })
                    .ToListAsync();
            });

            return cached ?? new List<LookupDto>();
        }
    }
}
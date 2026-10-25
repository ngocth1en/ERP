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
    public class AssetMiddleTypeService : IAssetMiddleTypeService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private static string LookupCacheKey(int bigTypeId) => $"asset_middle_types_lookup_{bigTypeId}";

        public AssetMiddleTypeService(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        /* ============================
           SEARCH BY BIG TYPE (TAB 1)
        ============================ */
        public async Task<PagedResult<AssetMiddleTypeListDto>> SearchAsync(
    AssetMiddleTypeSearchDto dto)
        {
            var query = _context.Asset_Middle_Types
                .AsNoTracking()
                .Where(x => x.Big_Type_Id == dto.BigTypeId);

            if (!string.IsNullOrWhiteSpace(dto.Keyword))
            {
                query = query.Where(x =>
                    x.Middle_Type_Name_EN.Contains(dto.Keyword) ||
                    x.Middle_Type_Name_VN.Contains(dto.Keyword));
            }

            if (dto.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == true);
            }

            if (!string.IsNullOrEmpty(dto.InventoryCheckType))
            {
                query = query.Where(x => x.Inventory_Check_Type == dto.InventoryCheckType);
            }

            if (!string.IsNullOrEmpty(dto.MachineType))
            {
                query = query.Where(x => x.Machine_Type == dto.MachineType);
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.Middle_Type_Name_EN)
                .Skip(dto.Skip)
                .Take(dto.Take)
                .Select(x => new AssetMiddleTypeListDto
                {
                    MiddleTypeId = x.Middle_Type_Id,
                    NameEN = x.Middle_Type_Name_EN,
                    NameVN = x.Middle_Type_Name_VN,
                    Machine_Type = x.Machine_Type,
                    Inventory_Check_Type = x.Inventory_Check_Type,
                    IsActive = true,
                    UpdatedAt = x.UpdatedAt
                })
                .ToListAsync();

            return new PagedResult<AssetMiddleTypeListDto>
            {
                Total = total,
                Items = items
            };
        }


        /* ============================
           CREATE
        ============================ */
        public async Task<int> CreateAsync(
    CreateAssetMiddleTypeDto dto,
    string username)
        {
            /* ================= VALIDATE ================= */

            if (dto.BigTypeId <= 0)
                throw new ArgumentException("BigTypeId không hợp lệ");

            if (string.IsNullOrWhiteSpace(dto.NameEN))
                throw new ArgumentException("Tên EN không được để trống");

            if (string.IsNullOrWhiteSpace(dto.NameVN))
                throw new ArgumentException("Tên VN không được để trống");

            /* ================= USER ================= */

            var userId = await GetUserId(username);

            /* ================= CHECK FK ================= */
            // Bắt buộc tồn tại BigType (tránh lỗi FK SQL)

            var existsBigType = await _context.Asset_Big_Types
                .AnyAsync(x => x.Big_Type_Id == dto.BigTypeId);

            if (!existsBigType)
                throw new InvalidOperationException($"BigTypeId = {dto.BigTypeId} không tồn tại");

            /* ================= CREATE ENTITY ================= */

            var entity = new Asset_Middle_Types
            {
                Middle_Type_Name_EN = dto.NameEN.Trim(),
                Middle_Type_Name_VN = dto.NameVN.Trim(),

                Machine_Type = dto.Machine_Type,                 // "1" | "2"
                Inventory_Check_Type = dto.Inventory_Check_Type, // "1" | "2"

                Big_Type_Id = dto.BigTypeId,                      // 🔥 CHỈ GÁN FK
                IsActive = true,

                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            /* ================= SAVE ================= */

            _context.Asset_Middle_Types.Add(entity);
            await _context.SaveChangesAsync();

            // ✅ Danh sách lookup theo BigTypeId này vừa thay đổi → xóa cache tương ứng
            _cache.Remove(LookupCacheKey(dto.BigTypeId));

            return entity.Middle_Type_Id;
        }


        /* ============================
           UPDATE
        ============================ */
        public async Task UpdateAsync(
            int id,
            UpdateAssetMiddleTypeDto dto,
            string username)
        {
            var userId = await GetUserId(username);
            var entity = await _context.Asset_Middle_Types
                .FirstOrDefaultAsync(x => x.Middle_Type_Id == id);

            if (entity == null)
                throw new Exception("Middle Type not found");

            entity.Middle_Type_Name_EN = dto.NameEN;
            entity.Middle_Type_Name_VN = dto.NameVN;
            entity.Machine_Type = dto.Machine_Type;
            entity.Inventory_Check_Type = dto.Inventory_Check_Type;
            entity.IsActive = dto.IsActive;

            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            _cache.Remove(LookupCacheKey(entity.Big_Type_Id));
        }

        /* ============================
           HIDE / SHOW (INSTEAD DELETE)
        ============================ */
        public async Task ToggleActiveAsync(int id, string username)
        {
            var userId = await GetUserId(username);
            var entity = await _context.Asset_Middle_Types
                .FirstOrDefaultAsync(x => x.Middle_Type_Id == id);

            if (entity == null)
                throw new Exception("Middle Type not found");

            entity.IsActive = !entity.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            _cache.Remove(LookupCacheKey(entity.Big_Type_Id));
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

        /// <summary>
        /// Lookup theo BigTypeId — dùng cho dropdown phụ thuộc (cascading select).
        /// ✅ Cache 15 phút theo từng bigTypeId để giảm tải DB khi nhiều user cùng mở form.
        /// </summary>
        public async Task<List<LookupDto>> GetByBigTypeAsync(int bigTypeId)
        {
            var cached = await _cache.GetOrCreateAsync(LookupCacheKey(bigTypeId), async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

                return await _context.Asset_Middle_Types
                    .AsNoTracking()
                    .Where(x => x.Big_Type_Id == bigTypeId && x.IsActive)
                    .OrderBy(x => x.Middle_Type_Name_VN)
                    .Select(x => new LookupDto
                    {
                        Id = x.Middle_Type_Id,
                        Name = x.Middle_Type_Name_VN
                    })
                    .ToListAsync();
            });

            return cached ?? new List<LookupDto>();
        }
    }
}
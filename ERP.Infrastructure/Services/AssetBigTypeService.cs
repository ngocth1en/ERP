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
    public class AssetBigTypeService : IAssetBigTypeService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private const string LookupCacheKey = "asset_big_types_lookup";

        public AssetBigTypeService(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        /* ================= SEARCH ================= */
        public async Task<PagedResult<AssetBigTypeListDto>> SearchAsync(
    SearchAssetBigTypeDto dto)
        {
            var query = _context.Asset_Big_Types.AsNoTracking()
                .Where(b => !b.IsDeleted)
                .GroupJoin(
                    _context.Users,
                    b => b.UpdatedBy,
                    u => u.Id,
                    (b, users) => new { b, users }
                )
                .SelectMany(
                    x => x.users.DefaultIfEmpty(),
                    (x, u) => new { b = x.b, u }
                );

            if (!string.IsNullOrWhiteSpace(dto.NameEN))
            {
                query = query.Where(x =>
                    x.b.Big_Type_Name_EN.Contains(dto.NameEN));
            }

            if (!string.IsNullOrWhiteSpace(dto.NameVN))
            {
                query = query.Where(x =>
                    x.b.Big_Type_Name_VN.Contains(dto.NameVN));
            }

            var total = await query.CountAsync();

            var rawItems = await query
                .OrderByDescending(x => x.b.UpdatedAt ?? x.b.CreatedAt)
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(x => new
                {
                    x.b.Big_Type_Id,
                    x.b.Big_Type_Name_EN,
                    x.b.Big_Type_Name_VN,
                    x.b.UpdatedAt,
                    UpdatedByName = x.u != null ? x.u.UserName : null
                })
                .ToListAsync();

            var items = rawItems
                .Select((x, index) => new AssetBigTypeListDto
                {
                    Stt = (dto.Page - 1) * dto.PageSize + index + 1,
                    BigTypeId = x.Big_Type_Id,
                    BigTypeNameEN = x.Big_Type_Name_EN,
                    BigTypeNameVN = x.Big_Type_Name_VN,
                    UpdatedAt = x.UpdatedAt,
                    UpdatedByName = x.UpdatedByName
                })
                .ToList();

            return new PagedResult<AssetBigTypeListDto>
            {
                Total = total,
                Items = items
            };
        }



        /* ================= CREATE ================= */
        public async Task CreateAsync(CreateAssetBigTypeDto dto, string userName)
        {
            var userId = await GetUserId(userName);

            var entity = new Asset_Big_Types
            {
                Big_Type_Name_EN = dto.NameEN,
                Big_Type_Name_VN = dto.NameVN,
                CreatedAt = DateTime.Now,
                CreatedBy = userId
            };

            _context.Asset_Big_Types.Add(entity);
            await _context.SaveChangesAsync();

            // ✅ Dữ liệu lookup vừa thay đổi → xóa cache để lần gọi tiếp theo load lại từ DB
            _cache.Remove(LookupCacheKey);
        }


        /* ================= UPDATE ================= */
        public async Task UpdateAsync(
    int id,
    UpdateAssetBigTypeDto dto,
    string userName)
        {
            var entity = await _context.Asset_Big_Types
                .FirstOrDefaultAsync(x => x.Big_Type_Id == id && !x.IsDeleted);

            if (entity == null)
                throw new Exception("Loại thiết bị không tồn tại");

            entity.Big_Type_Name_EN = dto.NameEN;
            entity.Big_Type_Name_VN = dto.NameVN;
            entity.UpdatedAt = DateTime.Now;
            entity.UpdatedBy = await GetUserId(userName);

            await _context.SaveChangesAsync();

            _cache.Remove(LookupCacheKey);
        }


        /* ================= DELETE (SOFT) ================= */
        public async Task DeleteAsync(int id, string userName)
        {
            var entity = await _context.Asset_Big_Types
                .FirstOrDefaultAsync(x => x.Big_Type_Id == id && !x.IsDeleted);

            if (entity == null)
                throw new Exception("Loại thiết bị không tồn tại");

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.UpdatedBy = await GetUserId(userName);

            await _context.SaveChangesAsync();

            _cache.Remove(LookupCacheKey);
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
        /// Danh sách lookup cho dropdown/filter — được gọi rất nhiều nơi (mỗi khi mở form, đổi tab...).
        /// ✅ Cache 15 phút trong RAM để giảm tải DB khi có nhiều request đồng thời.
        /// </summary>
        public async Task<List<LookupDto>> GetBigTypeAsync()
        {
            var cached = await _cache.GetOrCreateAsync(LookupCacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

                return await _context.Asset_Big_Types
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.Big_Type_Name_VN)
                    .Select(x => new LookupDto
                    {
                        Id = x.Big_Type_Id,
                        Name = x.Big_Type_Name_VN
                    })
                    .ToListAsync();
            });

            return cached ?? new List<LookupDto>();
        }
    }

}
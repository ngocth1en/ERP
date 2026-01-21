using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Asset.Model.Interfaces;
using ERP.Application.Common;
using ERP.Infrastructure.ERP.Domain.Entities;
using ERP.Infrastructure.ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
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

        public AssetBigTypeService(AppDbContext context)
        {
            _context = context;
        }

        /* ================= SEARCH ================= */
        public async Task<PagedResult<AssetBigTypeListDto>> SearchAsync(
    SearchAssetBigTypeDto dto)
        {
            var query = _context.Asset_Big_Types
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

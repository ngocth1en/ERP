using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Asset.Model.Interfaces;
using ERP.Application.Common;
using ERP.Domain.Entities.ASM;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
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

        public AssetMiddleTypeService(AppDbContext context)
        {
            _context = context;
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
                query = query.Where(x => x.IsActive == dto.IsActive.Value);
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
                    IsActive = x.IsActive,
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
            Guid userId)
        {
            var entity = new Asset_Middle_Type
            {
                Middle_Type_Name_EN = dto.NameEN,
                Middle_Type_Name_VN = dto.NameVN,
                Machine_Type = dto.Machine_Type,           // "1" | "0"
                Inventory_Check_Type = dto.Inventory_Check_Type,
                Big_Type_Id = dto.BigTypeId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.Asset_Middle_Types.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Middle_Type_Id;
        }

        /* ============================
           UPDATE
        ============================ */
        public async Task UpdateAsync(
            int id,
            UpdateAssetMiddleTypeDto dto,
            Guid userId)
        {
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
        }

        /* ============================
           HIDE / SHOW (INSTEAD DELETE)
        ============================ */
        public async Task ToggleActiveAsync(int id, Guid userId)
        {
            var entity = await _context.Asset_Middle_Types
                .FirstOrDefaultAsync(x => x.Middle_Type_Id == id);

            if (entity == null)
                throw new Exception("Middle Type not found");

            entity.IsActive = !entity.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = userId;

            await _context.SaveChangesAsync();
        }
    }
}

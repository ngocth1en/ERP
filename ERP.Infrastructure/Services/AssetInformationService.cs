
using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Asset.Model.Interfaces;
using ERP.Infrastructure.ERP.Domain.Entities;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Application.Asset.Information.Services
{
    public class AssetInformationService : IAssetInformationService
    {
        private readonly AppDbContext _context;

        public AssetInformationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AssetStatisticResultDto>> SearchStatisticAsync(
            AssetStatisticSearchDto dto)
        {
            var query = _context.Assets_Information
                .AsNoTracking()
                .Include(x => x.Model)
                    .ThenInclude(s => s.Small_Type)
                        .ThenInclude(m => m.Middle_Type)
                            .ThenInclude(b => b.Big_Type)
                .AsQueryable();

            #region FILTER

            if (!string.IsNullOrEmpty(dto.DepartmentId))
                query = query.Where(x => x.DepartmentId == dto.DepartmentId);

            if (!string.IsNullOrEmpty(dto.AssetNo))
                query = query.Where(x => x.AssetNo.Contains(dto.AssetNo));

            if (dto.Status.HasValue)
                query = query.Where(x => x.Status == dto.Status.Value);

            if (dto.MiddleTypeId.HasValue)
                query = query.Where(x =>
                    x.Model.Middle_Type_Id == dto.MiddleTypeId.Value);

            if (dto.BigTypeId.HasValue)
                query = query.Where(x =>
                    x.Model.Big_Type_Id == dto.BigTypeId.Value);

            #endregion

            var result = await query
                .GroupBy(x => new
                {
                    x.Model_Id,
                    BigName = x.Model.Big_Type.Big_Type_Name_VN,
                    MiddleName = x.Model.Middle_Type.Middle_Type_Name_VN,
                    SmallName = x.Model.Small_Type.Small_Type_Name_VN,
                    ModelName = x.Model.Model_Name
                })
                .Select((g) => new AssetStatisticResultDto
                {
                    AssetName = g.Key.BigName + " - " +
                                g.Key.MiddleName + " - " +
                                g.Key.SmallName,

                    ModelName = g.Key.ModelName,

                    TotalAssets = g.Count()
                })
                .OrderBy(x => x.AssetName)
                .ToListAsync();

            return result;
        }
    }
}

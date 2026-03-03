using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Common;
using ERP.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ERP.Application.Asset.Model.Interfaces
{
    public interface IAssetInformationService
    {
        Task<PagedResult<AssetStatisticResultDto>> SearchStatisticAsync(
            AssetStatisticSearchDto searchDto);
        Task<PagedResult<AssetByModelResultDto>> SearchAssetsByModelAsync(
            AssetByModelSearchDto dto);
        Task CreateAsync(CreateAssetDto dto, string username);
        Task UpdateAsync(string assetNo, UpdateAssetDto dto, string username);

        // kiểm kê
        Task<InventoryCheckResultDto> InventoryCheckAsync(InventoryCheckRequestDto dto, string username);

        // tổng hợp theo tháng
        Task<List<MonthlyInventoryDto>> GetMonthlyInventorySummaryAsync(int modelId, string? departmentId, int months = 6);
    }
}

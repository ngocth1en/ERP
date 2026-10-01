using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Interfaces
{
    public interface IAssetLiquidationService
    {
        Task<LiquidationOrderDto> CreateAsync(CreateLiquidationDto dto, string username);
        Task<LiquidationOrderDto> UpdateAsync(string id, UpdateLiquidationDto dto, string username);
        Task<LiquidationOrderDto> GetAsync(string id);
        Task<PagedResult<LiquidationOrderListDto>> SearchAsync(SearchLiquidationOrdersDto dto);

        Task<LiquidationOrderDto> ConfirmAsync(string id, string username);
        Task<LiquidationOrderDto> CancelAsync(string id, string username);

        Task<LiquidationOrderDto> AddAssetsAsync(string id, AddLiquidationAssetsDto dto, string username);
        Task RemoveAssetAsync(string id, int detailId, string username);

        /// <summary>Tìm tài sản đã báo phế (Confirmed) để tạo phiếu thanh lý</summary>
        Task<PagedResult<ScrappedAssetCandidateDto>> SearchScrappedAssetsAsync(SearchScrappedAssetsDto dto);
    }
}

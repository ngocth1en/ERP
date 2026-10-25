using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Interfaces
{
    public interface IAssetScrapService
    {
        // Phiếu báo phế
        Task<ScrapOrderDto> CreateAsync(CreateScrapDto dto, string username);
        Task<ScrapOrderDto> UpdateAsync(string id, UpdateScrapDto dto, string username);
        Task<ScrapOrderDto> GetAsync(string id);
        Task<PagedResult<ScrapOrderListDto>> SearchAsync(SearchScrapOrdersDto dto);

        // Xác nhận / Hủy
        Task<ScrapOrderDto> ConfirmAsync(string id, string username);
        Task<ScrapOrderDto> CancelAsync(string id, string username);

        // Chi tiết tài sản trong phiếu
        Task<ScrapOrderDto> AddAssetsAsync(string id, AddScrapAssetsDto dto, string username);
        Task RemoveAssetAsync(string id, int detailId, string username);

        // Tìm kiếm tài sản có thể báo phế
        Task<PagedResult<ScrapAssetCandidateDto>> SearchCandidateAssetsAsync(SearchScrapAssetsDto dto);
    }
}

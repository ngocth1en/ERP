using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Interfaces
{
    public interface IAssetHandoverService
    {
        // Phiếu bàn giao
        Task<HandoverOrderDto> CreateAsync(CreateHandoverDto dto, string username);
        Task<HandoverOrderDto> UpdateAsync(string id, UpdateHandoverDto dto, string username);
        Task<HandoverOrderDto> GetAsync(string id);
        Task<PagedResult<HandoverOrderListDto>> SearchAsync(SearchHandoverOrdersDto dto);

        // Xác nhận / Hủy
        Task<HandoverOrderDto> ConfirmAsync(string id, string username);
        Task<HandoverOrderDto> CancelAsync(string id, string username);

        // Chi tiết tài sản trong phiếu
        Task<HandoverOrderDto> AddAssetsAsync(string id, AddHandoverAssetsDto dto, string username);
        Task RemoveAssetAsync(string id, int detailId, string username);

        // Tìm kiếm tài sản có thể bàn giao
        Task<PagedResult<HandoverAssetCandidateDto>> SearchCandidateAssetsAsync(SearchHandoverAssetsDto dto);
    }
}

using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Interfaces
{
    public interface IAssetRepairService
    {
        Task<RepairOrderDto> CreateAsync(CreateRepairDto dto, string username);
        Task<RepairOrderDto> UpdateAsync(string id, UpdateRepairDto dto, string username);
        Task<RepairOrderDto> GetAsync(string id);
        Task<PagedResult<RepairOrderListDto>> SearchAsync(SearchRepairOrdersDto dto);

        /// <summary>Bắt đầu sửa chữa (Pending → InProgress)</summary>
        Task<RepairOrderDto> StartAsync(string id, string username);

        /// <summary>Hoàn thành sửa chữa → phục hồi tài sản (Is_Deleted = false)</summary>
        Task<RepairOrderDto> CompleteAsync(string id, string username);

        /// <summary>Hủy đơn sửa chữa</summary>
        Task<RepairOrderDto> CancelAsync(string id, string username);

        /// <summary>Tìm tài sản đã báo phế (Confirmed) chưa có đơn sửa chữa/thanh lý đang xử lý</summary>
        Task<PagedResult<ScrappedAssetCandidateDto>> SearchScrappedAssetsAsync(SearchScrappedAssetsDto dto);
        /// <summary>Lịch sử sửa chữa (tất cả đơn) theo bộ lọc tài sản/đơn vị/khoảng thời gian</summary>
        Task<PagedResult<RepairHistoryItemDto>> GetMaintenanceHistoryAsync(MaintenanceHistoryQueryDto dto);

        /// <summary>Thống kê số lần sửa chữa và tổng chi phí theo từng tài sản</summary>
        Task<PagedResult<AssetMaintenanceSummaryDto>> GetMaintenanceSummaryByAssetAsync(MaintenanceHistoryQueryDto dto);

        /// <summary>Thống kê tổng hợp toàn hệ thống (số đơn, tổng chi phí, top tài sản tốn kém nhất)</summary>
        Task<MaintenanceOverallStatisticDto> GetOverallStatisticAsync(DateTime? fromDate, DateTime? toDate, int topCount = 10);
    }
}

using ERP.Domain.Enum.ASM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Dtos
{
    // ─── REQUEST ──────────────────────────────────────────────

    public class CreateRepairDto
    {
        public string AssetNo { get; set; } = null!;
        /// <summary>Phiếu báo phế gốc (tuỳ chọn - nếu tạo từ báo phế)</summary>
        public string? ScrapId { get; set; }
        public string? RepairVendor { get; set; }
        public string? Reason { get; set; }
        public decimal? Cost { get; set; }
        public string? Note { get; set; }
    }

    public class UpdateRepairDto
    {
        public string? RepairVendor { get; set; }
        public string? Reason { get; set; }
        public decimal? Cost { get; set; }
        public string? Note { get; set; }
    }

    public class SearchRepairOrdersDto
    {
        public string? DepartmentId { get; set; }
        public string? AssetNo { get; set; }
        public RepairStatus? Status { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    // ─── RESPONSE ─────────────────────────────────────────────

    public class RepairOrderDto
    {
        public string Id { get; set; } = null!;
        public string DepartmentId { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public string AssetNo { get; set; } = null!;
        public string? AssetName { get; set; }
        public string? ModelName { get; set; }
        public string? ScrapId { get; set; }
        public string? RepairVendor { get; set; }
        public RepairStatus Status { get; set; }
        public string? Reason { get; set; }
        public decimal? Cost { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? CompletedByUserName { get; set; }
    }

    public class RepairOrderListDto
    {
        public string Id { get; set; } = null!;
        public string DepartmentId { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public string AssetNo { get; set; } = null!;
        public string? AssetName { get; set; }
        public RepairStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedByUserName { get; set; }
    }

    /// <summary>Tài sản đã báo phế (Confirmed), có thể tạo đơn sửa chữa/thanh lý</summary>
    public class ScrappedAssetCandidateDto
    {
        public string AssetNo { get; set; } = null!;
        public string AssetName { get; set; } = null!;
        public string ModelName { get; set; } = null!;
        public string ScrapId { get; set; } = null!;
        public string DepartmentId { get; set; } = null!;
        public DateTime ScrapConfirmedAt { get; set; }
    }

    public class SearchScrappedAssetsDto
    {
        public string? DepartmentId { get; set; }
        public string? AssetNo { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    // ─── BẢO TRÌ MÁY (LỊCH SỬ + THỐNG KÊ CHI PHÍ) ──────────────

    /// <summary>Bộ lọc tra cứu lịch sử sửa chữa / thống kê chi phí bảo trì</summary>
    public class MaintenanceHistoryQueryDto
    {
        /// <summary>Lọc theo 1 tài sản cụ thể (ưu tiên nếu có)</summary>
        public string? AssetNo { get; set; }
        public string? DepartmentId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public RepairStatus? Status { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    /// <summary>1 dòng lịch sử sửa chữa của 1 tài sản</summary>
    public class RepairHistoryItemDto
    {
        public string Id { get; set; } = null!;
        public string AssetNo { get; set; } = null!;
        public string? AssetName { get; set; }
        public string DepartmentId { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public string? RepairVendor { get; set; }
        public string? Reason { get; set; }
        public decimal? Cost { get; set; }
        public RepairStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? CompletedByUserName { get; set; }
    }

    /// <summary>Thống kê chi phí sửa chữa tổng hợp theo tài sản</summary>
    public class AssetMaintenanceSummaryDto
    {
        public string AssetNo { get; set; } = null!;
        public string? AssetName { get; set; }
        public string? ModelName { get; set; }
        public string DepartmentId { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;

        /// <summary>Tổng số lần đã tạo đơn sửa chữa</summary>
        public int TotalRepairCount { get; set; }

        /// <summary>Số lần sửa chữa đã hoàn thành</summary>
        public int CompletedRepairCount { get; set; }

        /// <summary>Tổng chi phí đã ghi nhận (tất cả trạng thái có Cost)</summary>
        public decimal TotalCost { get; set; }

        /// <summary>Ngày sửa chữa gần nhất</summary>
        public DateTime? LastRepairAt { get; set; }
    }

    /// <summary>Kết quả thống kê tổng hợp toàn hệ thống (cho dashboard)</summary>
    public class MaintenanceOverallStatisticDto
    {
        public int TotalRepairOrders { get; set; }
        public int CompletedRepairOrders { get; set; }
        public int InProgressRepairOrders { get; set; }
        public decimal TotalCost { get; set; }
        public List<AssetMaintenanceSummaryDto> TopAssetsByCost { get; set; } = new();
    }
}

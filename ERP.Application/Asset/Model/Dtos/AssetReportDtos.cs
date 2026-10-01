using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Dtos
{
    /// <summary>Loại kỳ báo cáo</summary>
    public enum ReportPeriodType
    {
        Month = 1,
        Quarter = 2,
        Year = 3
    }

    public class AssetReportSearchDto
    {
        public ReportPeriodType PeriodType { get; set; } = ReportPeriodType.Month;

        public int Year { get; set; } = DateTime.Now.Year;

        /// <summary>Bắt buộc nếu PeriodType = Month (1-12)</summary>
        public int? Month { get; set; }

        /// <summary>Bắt buộc nếu PeriodType = Quarter (1-4)</summary>
        public int? Quarter { get; set; }

        /// <summary>Loại thiết bị lớn</summary>
        public int? BigTypeId { get; set; }

        /// <summary>Loại thiết bị vừa</summary>
        public int? MiddleTypeId { get; set; }

        /// <summary>Loại thiết bị nhỏ</summary>
        public int? SmallTypeId { get; set; }

        /// <summary>Model cụ thể (tuỳ chọn)</summary>
        public int? ModelId { get; set; }

        /// <summary>Lọc theo đơn vị cụ thể (tuỳ chọn)</summary>
        public string? DepartmentId { get; set; }
    }

    /// <summary>1 dòng chi tiết: 1 đơn vị x 1 trạng thái</summary>
    public class AssetReportItemDto
    {
        public string DepartmentId { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public int Status { get; set; }
        public string StatusText { get; set; } = null!;
        public int Count { get; set; }
    }

    /// <summary>Tổng hợp theo đơn vị (dùng cho bảng pivot hiển thị UI)</summary>
    public class AssetReportByDepartmentDto
    {
        public string DepartmentId { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public int NotUsedCount { get; set; }     // Không sử dụng
        public int WaitingCount { get; set; }     // Chờ sử dụng
        public int InUseCount { get; set; }       // Đang sử dụng
        public int ScrapCount { get; set; }       // Phế
        public int LiquidatedCount { get; set; }  // Thanh lý
        public int TotalCount { get; set; }
    }

    public class AssetReportResultDto
    {
        public string PeriodLabel { get; set; } = null!; // VD: "Tháng 10/2026", "Quý 4/2026", "Năm 2026"
        public List<AssetReportByDepartmentDto> Items { get; set; } = new();
        public int GrandTotal { get; set; }
    }
}

using ERP.Domain.Enum.ASM;
using ERP.Infrastructure.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities.ASM
{
    /// <summary>Đơn sửa chữa tài sản (tạo từ tài sản đã báo phế)</summary>
    public class Asset_Repairs
    {
        /// <summary>Mã đơn sửa chữa, VD: RP2026010001</summary>
        public string Id { get; set; } = null!;

        /// <summary>Đơn vị đang quản lý tài sản</summary>
        public string DepartmentId { get; set; } = null!;

        /// <summary>Mã tài sản cần sửa chữa</summary>
        public string AssetNo { get; set; } = null!;

        /// <summary>Phiếu báo phế gốc (nếu tạo từ báo phế)</summary>
        public string? ScrapId { get; set; }

        /// <summary>Đơn vị/nhà cung cấp sửa chữa (tuỳ chọn, text tự do)</summary>
        public string? RepairVendor { get; set; }

        /// <summary>Trạng thái: 1=Nháp, 2=Chờ duyệt, 3=Đang sửa, 4=Hoàn thành, 5=Hủy</summary>
        public RepairStatus Status { get; set; } = RepairStatus.Draft;

        /// <summary>Lý do / mô tả hư hỏng</summary>
        public string? Reason { get; set; }

        /// <summary>Chi phí sửa chữa (tuỳ chọn)</summary>
        public decimal? Cost { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        /// <summary>Ngày hoàn thành sửa chữa</summary>
        public DateTime? CompletedAt { get; set; }
        public Guid? CompletedBy { get; set; }

        // Navigation
        public virtual Departments Department { get; set; } = null!;
        public virtual Assets_Information Asset { get; set; } = null!;
        public virtual Asset_Scraps? Scrap { get; set; }
    }
}

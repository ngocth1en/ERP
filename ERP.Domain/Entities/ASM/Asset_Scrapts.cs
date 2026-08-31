using ERP.Domain.Enum.ASM;
using ERP.Infrastructure.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities.ASM
{
    /// <summary>Phiếu báo phế tài sản</summary>
    public class Asset_Scraps
    {
        /// <summary>Mã phiếu báo phế, VD: SC2026010001</summary>
        public string Id { get; set; } = null!;

        /// <summary>Đơn vị đang sử dụng tài sản báo phế</summary>
        public string DepartmentId { get; set; } = null!;

        /// <summary>Vị trí lưu trữ (tuỳ chọn)</summary>
        public string? StorageLocationNo { get; set; }

        /// <summary>Trạng thái: 1=Nháp, 2=Chờ duyệt, 3=Hoàn thành, 4=Từ chối, 5=Hủy</summary>
        public ScrapStatus Status { get; set; } = ScrapStatus.Draft;

        /// <summary>Lý do báo phế</summary>
        public string? Reason { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        /// <summary>Ngày xác nhận hoàn thành báo phế</summary>
        public DateTime? ConfirmedAt { get; set; }
        public Guid? ConfirmedBy { get; set; }

        // Navigation
        public virtual Departments Department { get; set; } = null!;
        public virtual StorageLocations? StorageLocation { get; set; }
        public virtual ICollection<Asset_Scrap_Details> Details { get; set; } = new List<Asset_Scrap_Details>();
    }
}

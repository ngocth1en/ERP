using ERP.Domain.Enum.ASM;
using ERP.Infrastructure.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities.ASM
{
    /// <summary>Phiếu thanh lý tài sản (tạo từ tài sản đã báo phế)</summary>
    public class Asset_Liquidations
    {
        /// <summary>Mã phiếu thanh lý, VD: LQ2026010001</summary>
        public string Id { get; set; } = null!;

        /// <summary>Đơn vị đang quản lý tài sản</summary>
        public string DepartmentId { get; set; } = null!;

        /// <summary>Trạng thái: 1=Nháp, 2=Chờ duyệt, 3=Hoàn thành, 4=Hủy</summary>
        public LiquidationStatus Status { get; set; } = LiquidationStatus.Draft;

        /// <summary>Hình thức thanh lý (VD: Bán, Tiêu hủy, Cho tặng...)</summary>
        public string? LiquidationMethod { get; set; }

        /// <summary>Giá trị thu hồi (nếu bán)</summary>
        public decimal? RecoveredAmount { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        /// <summary>Ngày xác nhận hoàn thành thanh lý</summary>
        public DateTime? ConfirmedAt { get; set; }
        public Guid? ConfirmedBy { get; set; }

        // Navigation
        public virtual Departments Department { get; set; } = null!;
        public virtual ICollection<Asset_Liquidation_Details> Details { get; set; } = new List<Asset_Liquidation_Details>();
    }
}

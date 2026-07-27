using ERP.Domain.Enum.ASM;
using ERP.Infrastructure.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities.ASM
{
    public class Asset_Handovers
    {
        /// <summary>Mã phiếu bàn giao, VD: HO2026010001</summary>
        public string Id { get; set; } = null!;

        /// <summary>Đơn vị bàn giao (bên giao)</summary>
        public string FromDepartmentId { get; set; } = null!;

        /// <summary>Đơn vị nhận (bên nhận)</summary>
        public string ToDepartmentId { get; set; } = null!;

        /// <summary>Vị trí lưu trữ nguồn (tuỳ chọn)</summary>
        public string? FromStorageLocationNo { get; set; }

        /// <summary>Vị trí lưu trữ đích (tuỳ chọn)</summary>
        public string? ToStorageLocationNo { get; set; }

        /// <summary>Trạng thái: 1=Nháp, 2=Chờ duyệt, 3=Hoàn thành, 4=Từ chối, 5=Hủy</summary>
        public HandoverStatus Status { get; set; } = HandoverStatus.Draft;

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        /// <summary>Ngày xác nhận hoàn thành bàn giao</summary>
        public DateTime? ConfirmedAt { get; set; }
        public Guid? ConfirmedBy { get; set; }

        // Navigation
        public virtual Departments FromDepartment { get; set; } = null!;
        public virtual Departments ToDepartment { get; set; } = null!;
        public virtual StorageLocations? FromStorageLocation { get; set; }
        public virtual StorageLocations? ToStorageLocation { get; set; }
        public virtual ICollection<Asset_Handover_Details> Details { get; set; } = new List<Asset_Handover_Details>();
    }
}

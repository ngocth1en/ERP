using ERP.Infrastructure.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities.ASM
{
    public class Asset_Handover_Details
    {
        public int Id { get; set; }

        public string HandoverId { get; set; } = null!;

        public string AssetNo { get; set; } = null!;

        /// <summary>Snapshot đơn vị đang sử dụng trước khi bàn giao</summary>
        public string PreviousUseDepartmentId { get; set; } = null!;

        /// <summary>Snapshot vị trí lưu trữ trước khi bàn giao</summary>
        public string PreviousStorageLocationNo { get; set; } = null!;

        public string? Note { get; set; }

        // Navigation
        public virtual Asset_Handovers Handover { get; set; } = null!;
        public virtual Assets_Information Asset { get; set; } = null!;
    }
}

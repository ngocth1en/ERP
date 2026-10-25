using ERP.Infrastructure.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities.ASM
{
    /// <summary>Chi tiết tài sản trong phiếu thanh lý</summary>
    public class Asset_Liquidation_Details
    {
        public int Id { get; set; }

        public string LiquidationId { get; set; } = null!;

        public string AssetNo { get; set; } = null!;

        /// <summary>Phiếu báo phế gốc (nếu có)</summary>
        public string? ScrapId { get; set; }

        /// <summary>Snapshot đơn vị sử dụng trước khi thanh lý</summary>
        public string PreviousUseDepartmentId { get; set; } = null!;

        public string? Note { get; set; }

        // Navigation
        public virtual Asset_Liquidations Liquidation { get; set; } = null!;
        public virtual Assets_Information Asset { get; set; } = null!;
    }
}

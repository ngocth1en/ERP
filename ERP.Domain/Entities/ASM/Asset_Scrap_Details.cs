using ERP.Infrastructure.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities.ASM
{
    /// <summary>Chi tiết tài sản trong phiếu báo phế</summary>
    public class Asset_Scrap_Details
    {
        public int Id { get; set; }

        public string ScrapId { get; set; } = null!;

        public string AssetNo { get; set; } = null!;

        /// <summary>Snapshot đơn vị sử dụng trước khi báo phế</summary>
        public string PreviousUseDepartmentId { get; set; } = null!;

        /// <summary>Snapshot vị trí lưu trữ trước khi báo phế</summary>
        public string PreviousStorageLocationNo { get; set; } = null!;

        public string? Note { get; set; }

        // Navigation
        public virtual Asset_Scraps Scrap { get; set; } = null!;
        public virtual Assets_Information Asset { get; set; } = null!;
    }
}

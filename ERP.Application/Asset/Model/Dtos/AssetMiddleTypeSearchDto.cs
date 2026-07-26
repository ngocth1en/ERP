using ERP.Domain.Enum.ASM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Dtos
{
    public class AssetMiddleTypeSearchDto
    {
        /* ========== FILTER ========== */
        public int BigTypeId { get; set; } = default!;

        public string? Keyword { get; set; }

        public bool? IsActive { get; set; }

        public string? InventoryCheckType { get; set; }

        /// <summary>
        /// "1" = Máy, "0" = CCDC
        /// </summary>
        public string? MachineType { get; set; }

        /* ========== PAGING ========== */
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public int Skip => (Page - 1) * PageSize;
        public int Take => PageSize;
    }


}

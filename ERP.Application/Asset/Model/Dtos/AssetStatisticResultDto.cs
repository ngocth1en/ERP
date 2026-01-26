using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Dtos
{
    public class AssetStatisticResultDto
    {
        /// <summary>
        /// Tên tài sản = Big + Middle + Small
        /// </summary>
        public int ModelId { get; set; }
        public string? DepartmentId { get; set; }
        public string AssetName { get; set; } = default!;

        public string ModelName { get; set; } = default!;

        public int TotalAssets { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Dtos
{
    public class AssetByModelResultDto
    {

        public string AssetNo { get; set; } = default!;

        public string AssetName { get; set; } = default!;   // Big - Middle - Small

        public string ModelName { get; set; } = default!;

        public string DepartmentId { get; set; } = default!;

        public int Status { get; set; }
    }

}

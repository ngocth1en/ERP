using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Dtos
{
    public class AssetByModelSearchDto
    {
        public int ModelId { get; set; }   

        public string? AssetNo { get; set; }
        public string? DepartmentId { get; set; }
        public int? Status { get; set; }

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Dtos
{
    public class AssetModelSearchDto
    {
        public int? BigTypeId { get; set; }
        public int? MiddleTypeId { get; set; }
        public int? SmallTypeId { get; set; }

        public string? ModelName { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public int Skip => (Page - 1) * PageSize;
        public int Take => PageSize;
    }

}

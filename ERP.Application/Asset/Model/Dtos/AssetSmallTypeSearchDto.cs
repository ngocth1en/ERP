using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Dtos
{
    public class AssetSmallTypeSearchDto
    {
        public int MiddleTypeId { get; set; }

        public string? Keyword { get; set; }

        public bool? IsActive { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public int Skip => (Page - 1) * PageSize;
        public int Take => PageSize;
    }

}

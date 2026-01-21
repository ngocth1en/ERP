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

        public int Skip { get; set; }

        public int Take { get; set; }
    }

}

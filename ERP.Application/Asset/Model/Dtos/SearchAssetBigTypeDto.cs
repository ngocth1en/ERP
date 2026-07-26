using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Dtos
{
    public class SearchAssetBigTypeDto
    {
        public string? NameEN { get; set; }
        public string? NameVN { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Dtos
{
    public class AssetBigTypeListDto
    {
        public int Stt { get; set; }
        public int BigTypeId { get; set; } = default!;
        public string BigTypeNameEN { get; set; } = default!;
        public string BigTypeNameVN { get; set; } = default!;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedByName { get; set; }
    }

}

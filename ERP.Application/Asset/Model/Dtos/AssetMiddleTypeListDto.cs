using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Dtos
{
    public class AssetMiddleTypeListDto
    {
        public int MiddleTypeId { get; set; }
        public string NameEN { get; set; } = default!;
        public string NameVN { get; set; } = default!;
        public string Machine_Type { get; set; } = default!;
        public string Inventory_Check_Type { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedByName { get; set; }
    }

}

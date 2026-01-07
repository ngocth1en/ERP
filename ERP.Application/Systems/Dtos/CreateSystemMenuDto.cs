using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Systems.Dtos
{
    public class CreateSystemMenuDto
    {
        public string Code { get; set; } = default!;        // ASSET001
        public string SystemCode { get; set; } = default!;  // ASSET
        public string GroupCode { get; set; } = default!;   // ASSET-MANAGEMENT
        public string Name { get; set; } = default!;
        public string? Route { get; set; }
        public int OrderNo { get; set; }
    }

}

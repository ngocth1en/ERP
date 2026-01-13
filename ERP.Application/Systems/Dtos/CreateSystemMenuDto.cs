using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Systems.Dtos
{
    public class CreateSystemMenuDto
    {
        public string Code { get; set; } = default!;
        public string SystemCode { get; set; } = default!;
        public string? ParentCode { get; set; } // null = menu cha
        public string Name { get; set; } = default!;
        public string? Route { get; set; }
        public int OrderNo { get; set; }
    }


}

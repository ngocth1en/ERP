using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Systems.Dtos
{
    public class SidebarMenuManageDto
    {
        public string Key { get; set; } = default!;
        public string Label { get; set; } = default!;
        public bool IsUsing { get; set; }

        public List<SidebarMenuManageDto>? Children { get; set; }
    }

}

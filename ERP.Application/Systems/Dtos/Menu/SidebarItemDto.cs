using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Systems.Dtos.Menu
{
    public class SidebarItemDto
    {
        public string Key { get; set; } = default!;
        public string Label { get; set; } = default!;
        public string? Path { get; set; }
        public string? Permission { get; set; }
        public List<SidebarItemDto>? Children { get; set; }
    }

}

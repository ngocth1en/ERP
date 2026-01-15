using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Systems.Dtos.Permission
{
    public class BulkUserMenuPermissionDto
    {
        public string UserName { get; set; }
        public string SystemCode { get; set; } = default!;

        public List<MenuPermissionItemDto> Menus { get; set; } = new();
    }

    public class MenuPermissionItemDto
    {
        public string MenuCode { get; set; } = default!;
        public string PermissionMask { get; set; } = "1000000000";
    }

}

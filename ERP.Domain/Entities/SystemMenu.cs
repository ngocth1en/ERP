using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    public class SystemMenu
    {
        public string Code { get; set; } = default!; // ASSET001
        public string SystemCode { get; set; } = default!; // ASSET
        public string? ParentCode { get; set; }
        public SystemMenu? Parent { get; set; }
        public ICollection<SystemMenu> Children { get; set; } = new List<SystemMenu>();
        public string Name { get; set; } = default!;
        public string? Route { get; set; }
        public int OrderNo { get; set; }
        public bool IsUsing { get; set; } = true; // 👈 THÊM
        public SystemModule SystemModule { get; set; } = default!;
        public ICollection<UserMenuPermission> UserMenuPermissions { get; set; } = new List<UserMenuPermission>();
    }

}

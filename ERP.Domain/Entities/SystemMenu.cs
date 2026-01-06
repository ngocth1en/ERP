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
        public string GroupCode { get; set; } = default!; // ASSET-MANAGEMENT
        public string Name { get; set; } = default!;
        public string? Route { get; set; }
        public int OrderNo { get; set; }

        public SystemModule SystemModule { get; set; } = default!;
        public ICollection<UserMenuPermission> UserMenuPermissions { get; set; } = new List<UserMenuPermission>();
    }

}

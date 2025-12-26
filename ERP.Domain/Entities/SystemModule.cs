using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    public class SystemModule
    {
        public string Code { get; set; } = null!;   // ERP, HRM, WMS...
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }

        public ICollection<UserSystemPermission> UserSystemPermissions { get; set; }
            = new List<UserSystemPermission>();
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    public class SystemModule
    {
        public string Code { get; set; } = default!; // ASM, HRM...
        public string Name { get; set; } = default!;

        public ICollection<UserSystem> UserSystems { get; set; } = new List<UserSystem>();
    }


}

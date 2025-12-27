using ERP.Domain.Entities.ASM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    public class Department
    {
        public string Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;

        // 🔹 Asset quản lý
        public ICollection<Assets_Information> ManagedAssets { get; set; }
            = new List<Assets_Information>();

        // 🔹 Asset sử dụng
        public ICollection<Assets_Information> UsedAssets { get; set; }
            = new List<Assets_Information>();
        public ICollection<User> Users { get; set; } = new List<User>();
    }

}

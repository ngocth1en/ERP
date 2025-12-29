using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities.ASM
{
    public class StorageLocation
    {
        public string StorageLocationNo { get; set; }
        public string NameEN { get; set; }
        public string NameVN { get; set; }
        public string DepartmentId { get; set; }
        public Department Department { get; set; } = default!;
        public int inUse { get; set; }
        public ICollection<Assets_Information> Assets { get; set; }
        = new List<Assets_Information>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.StorageLocation.Dtos.StorageLocation
{
    public class CreateStorageLocationDto
    {
        public int Status { get; set; }
        public int Floor { get; set; }
        public string Name { get; set; } = default!;
        public string DepartmentId { get; set; } = default!;
        public int InUse { get; set; }
    }

}

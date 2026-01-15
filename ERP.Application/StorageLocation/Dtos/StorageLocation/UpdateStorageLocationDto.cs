using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.StorageLocation.Dtos.StorageLocation
{
    public class UpdateStorageLocationDto
    {
        public string Name { get; set; } = default!;
        public int Status { get; set; }
        public int InUse { get; set; }
    }

}

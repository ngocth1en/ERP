using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.StorageLocation.Dtos.StorageLocation
{
    public class StorageLocationListDto
    {
        public int STT { get; set; }
        public string StorageLocationNo { get; set; } = default!;
        public string NameVN { get; set; } = default!;
        public string DepartmentName { get; set; } = default!;
        public int Status { get; set; }
        public int InUse { get; set; }
    }


}

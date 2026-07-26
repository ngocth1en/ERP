using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    public class UpdateAssetDto
    {
        public string DepartmentId { get; set; } = null!;
        public string UseDepartmentId { get; set; } = null!;
        public int Model_Id { get; set; }
        public int Status { get; set; }
        public string StorageLocationNo { get; set; } = null!;
    }

}

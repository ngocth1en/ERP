using ERP.Domain.Entities.Common;
using ERP.Domain.Enum.ASM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities.ASM
{
    public class Assets_Information : BaseEntity
    {
        public string AssetNo { get; set; }
        public string QRCode { get; set; }

        // 🔹 Bộ phận QUẢN LÝ tài sản
        public string DepartmentId { get; set; }
        public Department Department { get; set; } = default!;

        // 🔹 Bộ phận SỬ DỤNG tài sản
        public string UseDepartmentId { get; set; }
        public Department UseDepartment { get; set; } = default!;
        public int Model_Id { get; set; }
        public Asset_Model AssetModel { get; set; }
        public AssetStatus Status { get; set; }
    }

}

using ERP.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities.ASM
{
    public class Asset_Small_Type:BaseEntity
    {
        public string Small_Type_Id { get; set; }
        public string Small_Type_Name_EN { get; set; }
        public string Small_Type_Name_VN { get; set; }
        public string Middle_Type_Id { get; set; } = default!;

        public Asset_Middle_Type Asset_Middle_Type { get; set; } = default!;
        public ICollection<Asset_Model> Models { get; set; } = new List<Asset_Model>();
    }
}

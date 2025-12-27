using ERP.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities.ASM
{
    public class Asset_Middle_Type:BaseEntity
    {
        public string Middle_Type_Id { get; set; }
        public string Middle_Type_Name_EN { get; set; }
        public string Middle_Type_Name_VN { get; set; }
        public string Machine_Type { get; set; }
        public string Big_Type_Id { get; set; } = default!;
        public Asset_Big_Type Asset_Big_Type { get; set; } = default!;
        public ICollection<Asset_Small_Type> SmallTypes { get; set; } = new List<Asset_Small_Type>();
    }
}

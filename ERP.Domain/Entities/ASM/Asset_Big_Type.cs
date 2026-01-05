using ERP.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities.ASM
{
    public class Asset_Big_Type :BaseEntity
    {
        public string Big_Type_Id { get; set; }
        public string Big_Type_Name_EN { get; set; }
        public string Big_Type_Name_VN { get; set; }
        public ICollection<Asset_Middle_Type> MiddleTypes { get; set; } = new List<Asset_Middle_Type>();
    }
}

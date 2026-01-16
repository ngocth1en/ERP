using ERP.Domain.Entities.Common;
using ERP.Domain.Enum.ASM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities.ASM
{
    public class Asset_Middle_Type:BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Middle_Type_Id { get; set; }
        public string Middle_Type_Name_EN { get; set; }
        public string Middle_Type_Name_VN { get; set; }
        public string Inventory_Check_Type { get; set; } = default!;
        public string Machine_Type { get; set; } = "0";
        public int Big_Type_Id { get; set; } = default!;
        public Asset_Big_Type Asset_Big_Type { get; set; } = default!;
        public bool IsActive { get; set; } = true;
        public ICollection<Asset_Small_Type> SmallTypes { get; set; } = new List<Asset_Small_Type>();
    }
}

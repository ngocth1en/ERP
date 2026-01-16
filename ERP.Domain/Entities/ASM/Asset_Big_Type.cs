using ERP.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities.ASM
{
    public class Asset_Big_Type :BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Big_Type_Id { get; set; }
        public string Big_Type_Name_EN { get; set; }
        public string Big_Type_Name_VN { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<Asset_Middle_Type> MiddleTypes { get; set; } = new List<Asset_Middle_Type>();
    }
}

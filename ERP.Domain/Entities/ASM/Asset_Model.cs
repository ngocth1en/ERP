using ERP.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities.ASM
{
    public class Asset_Model : BaseEntity
    {
        public int Model_Id { get; set; }  // PK

        public string Model_Name { get; set; } = default!;

        // FK Big
        public string Big_Type_Id { get; set; } = default!;
        public Asset_Big_Type Asset_Big_Type { get; set; } = default!;

        // FK Middle
        public string Middle_Type_Id { get; set; } = default!;
        public Asset_Middle_Type Asset_Middle_Type { get; set; } = default!;

        // FK Small
        public string Small_Type_Id { get; set; } = default!;
        public Asset_Small_Type Asset_Small_Type { get; set; } = default!;

        public byte[] ImageData { get; set; } = default!;

        public ICollection<Assets_Information> Assets { get; set; } = new List<Assets_Information>();
    }


}

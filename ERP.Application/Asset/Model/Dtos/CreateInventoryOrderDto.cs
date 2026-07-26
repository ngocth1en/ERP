using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Dtos
{
    public class CreateInventoryOrderDto
    {
        public int ModelId { get; set; }
        public string? DepartmentId { get; set; }
        public string? CheckLocationNo { get; set; }
        public string Inventory_Type { get; set; } = "1";
        public string? Note { get; set; }
    }
}

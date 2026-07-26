using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;

namespace ERP.Application.Asset.Model.Dtos
{
    public class UpdateInventoryOrderDto
    {
        public string? DepartmentId { get; set; }
        public string? Note { get; set; }
        public List<InventoryOrderDetailDto>? Details { get; set; }
    }
}

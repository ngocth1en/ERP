using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;

namespace ERP.Application.Asset.Model.Dtos
{
    public class InventoryOrderDto
    {
        public string Id { get; set; } = default!;
        public int ModelId { get; set; }
        public string? DepartmentId { get; set; }
        public string? CheckLocationNo { get; set; }
        public string InventoryType { get; set; } = default!;
        public int ExpectedCount { get; set; }
        public int Counted { get; set; }
        public int MissingCount { get; set; }
        public int UnexpectedCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Note { get; set; }
        public List<InventoryOrderDetailDto> Details { get; set; } = new();
    }
}

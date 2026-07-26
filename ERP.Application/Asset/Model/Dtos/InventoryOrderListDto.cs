using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ERP.Application.Asset.Model.Dtos
{
    public class InventoryOrderListDto
    {
        public string Id { get; set; } = default!;
        public int ModelId { get; set; }
        public string? DepartmentId { get; set; }
        public string? CheckLocationNo { get; set; }
        public string InventoryType { get; set; } = default!;
        public int ExpectedCount { get; set; }
        public int Counted { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedByUserName { get; set; }
    }
}

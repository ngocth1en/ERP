using System;
using System.Collections.Generic;

namespace ERP.Infrastructure.ERP.Domain.Entities
{
    public partial class Asset_Inventories
    {
        public Guid Id { get; set; }
        public int Model_Id { get; set; }
        public string? DepartmentId { get; set; }
        public string Inventory_Type { get; set; } = null!; // "1" / "2"
        public int ExpectedCount { get; set; }
        public int Counted { get; set; }
        public int MissingCount { get; set; }
        public int UnexpectedCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public string? Note { get; set; }

        public virtual ICollection<Asset_Inventory_Details> Details { get; set; } = new List<Asset_Inventory_Details>();
    }
}
using System;

namespace ERP.Infrastructure.ERP.Domain.Entities
{
    public partial class Asset_Inventory_Details
    {
        public Guid Id { get; set; }
        public Guid InventoryId { get; set; }
        public string AssetNo { get; set; } = null!;
        public string? QRCode { get; set; }
        public string Status { get; set; } = null!; // "FOUND", "MISSING", "UNEXPECTED"
        public string? Note { get; set; }

        public virtual Asset_Inventories Inventory { get; set; } = null!;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Dtos
{
    public class InventoryOrderDetailDto
    {
        public string Id { get; set; } = default!;
        public string AssetNo { get; set; } = default!;
        public string? QRCode { get; set; }
        public string Status { get; set; } = default!;
        public string? Note { get; set; }
    }
}

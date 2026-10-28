using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Dtos
{
    public class AssetModelListDto
    {
        public int ModelId { get; set; }
        public int BigTypeId { get; set; }
        public int MiddleTypeId { get; set; }
        public int SmallTypeId { get; set; }
        public string BigTypeNameVN { get; set; } = default!;
        public string MiddleTypeNameVN { get; set; } = default!;
        public string SmallTypeNameVN { get; set; } = default!;

        public string AssetFullName { get; set; } = default!;
        // Big + Middle + Small

        public string ModelName { get; set; } = default!;

        public byte[]? ImageData { get; set; }
        public string? Model3DUrl { get; set; }

        public string MachineType { get; set; } = default!;
        public string InventoryCheckType { get; set; } = default!;

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = default!;

        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Dtos
{
    public class CreateAssetModelDto
    {
        public int BigTypeId { get; set; }
        public int MiddleTypeId { get; set; }
        public int SmallTypeId { get; set; }

        public string ModelName { get; set; } = default!;

        public byte[]? ImageData { get; set; }
        public string? Model3DUrl { get; set; }
    }
}

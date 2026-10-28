using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Dtos
{
    public class UpdateAssetModelDto
    {
        public string ModelName { get; set; } = default!;

        public byte[]? ImageData { get; set; }

        public bool UpdateImage { get; set; }
        public string? Model3DUrl { get; set; }

        public bool UpdateModel3D { get; set; }
    }
}

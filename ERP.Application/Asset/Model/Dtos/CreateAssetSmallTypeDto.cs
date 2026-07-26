using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Asset.Model.Dtos
{
    public class CreateAssetSmallTypeDto
    {
        public int MiddleTypeId { get; set; }

        public string NameEN { get; set; } = default!;

        public string NameVN { get; set; } = default!;
    }

}

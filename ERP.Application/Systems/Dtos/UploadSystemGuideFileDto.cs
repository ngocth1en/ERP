using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Systems.Dtos
{
    public class UploadSystemGuideFileDto
    {
        public string SystemCode { get; set; } = default!;
        public string FileName { get; set; } = default!;
        public Stream FileStream { get; set; } = default!;
    }

}

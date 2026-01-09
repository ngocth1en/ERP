using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Systems.Dtos
{
    public class SystemGuideFileDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = default!;
        public DateTime UploadedAt { get; set; }
    }

}

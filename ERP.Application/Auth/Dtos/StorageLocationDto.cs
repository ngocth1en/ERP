using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Auth.Dtos
{
    public class StorageLocationDto
    {
        public string StorageLocationNo { get; set; }
        public string NameEN { get; set; }
        public string NameVN { get; set; }

        public int Status { get; set; }
        public string StatusText { get; set; }

        public int InUse { get; set; }
    }

}

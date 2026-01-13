using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Systems.Dtos
{
    public class UpdateSystemMenuStatusDto
    {
        public string Code { get; set; } = default!;
        public bool IsUsing { get; set; }
    }

}

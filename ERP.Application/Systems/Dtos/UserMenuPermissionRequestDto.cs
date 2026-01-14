using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Systems.Dtos
{
    public class UserMenuPermissionRequestDto
    {
        public string UserName { get; set; } = default!;
        public string SystemCode { get; set; } = default!;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

}

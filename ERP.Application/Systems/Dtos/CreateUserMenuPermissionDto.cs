using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Systems.Dtos
{
    public class CreateUserMenuPermissionDto
    {
        public string UserName { get; set; }
        public string MenuCode { get; set; } = default!;
        public string PermissionMask { get; set; } = "1000000000";
    }

}

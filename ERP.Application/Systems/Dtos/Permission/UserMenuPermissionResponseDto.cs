using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Systems.Dtos.Permission
{
    public class UserMenuPermissionResponseDto
    {
        public string MenuCode { get; set; } = default!;
        public string MenuName { get; set; } = default!;
        public string PermissionMask { get; set; } = default!;
    }

}

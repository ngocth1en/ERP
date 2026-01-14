using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Systems.Dtos
{
    public class UpdateUserMenuPermissionDto
    {
        public string UserName { get; set; } = default!;
        public string SystemCode { get; set; } = default!;
        public string MenuCode { get; set; } = default!;
        public string PermissionMask { get; set; } = default!;
    }

}

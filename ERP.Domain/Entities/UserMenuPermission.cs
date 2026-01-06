using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    public class UserMenuPermission
    {
        public Guid UserId { get; set; }
        public string MenuCode { get; set; } = default!;

        // 10 bit như bạn định nghĩa
        public string PermissionMask { get; set; } = "1000000000";

        public User User { get; set; } = default!;
        public SystemMenu SystemMenu { get; set; } = default!;
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    public class UserSystem
    {
        public Guid UserId { get; set; }
        public string SystemCode { get; set; } = default!;

        // Quyền hệ thống (hiển thị / ẩn combobox)
        public bool IsAllowed { get; set; }

        // Quyền thao tác (bitmask 10 ký tự)
        public string PermissionMask { get; set; } = "1000000000";

        public User User { get; set; } = default!;
        public SystemModule SystemModule { get; set; } = default!;
    }

}

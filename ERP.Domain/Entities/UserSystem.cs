using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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


        public User User { get; set; } = default!;
        public SystemModule SystemModule { get; set; } = default!;
    }

}

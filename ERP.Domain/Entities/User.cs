using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string UserName { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        public bool IsActive { get; set; }

        public ICollection<UserSystemPermission> UserSystemPermissions { get; set; }
            = new List<UserSystemPermission>();
    }

}

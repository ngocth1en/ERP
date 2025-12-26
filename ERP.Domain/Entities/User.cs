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
        public string UserName { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public Guid DepartmentId { get; set; }
        public Department Department { get; set; } = default!;
        public ICollection<UserSystem> UserSystems { get; set; } = new List<UserSystem>();
    }

}

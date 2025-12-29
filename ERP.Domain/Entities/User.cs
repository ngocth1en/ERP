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
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string BankName { get; set; }
        public string BankNum { get; set; }
        public string UserName { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public bool MustChangePassword { get; set; } = true;

        public string DepartmentId { get; set; }
        public Department Department { get; set; } = default!;
        public ICollection<UserSystem> UserSystems { get; set; } = new List<UserSystem>();
    }

}

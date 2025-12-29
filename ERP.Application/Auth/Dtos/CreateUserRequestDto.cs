using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Auth.Dtos
{
    public class CreateUserRequestDto
    {
        public string DepartmentId { get; set; } = default!;

        public string FName { get; set; } = default!;
        public string LName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string BankName { get; set; } = default!;
        public string BankNum { get; set; } = default!;
    }


}

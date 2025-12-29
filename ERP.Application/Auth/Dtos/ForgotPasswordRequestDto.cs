using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Auth.Dtos
{
    public class ForgotPasswordRequestDto
    {
        public string Email { get; set; } = default!;
    }

}

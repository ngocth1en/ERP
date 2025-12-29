using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Auth.Dtos
{
    public class UserCreatedResponseDto
    {
        public string UserName { get; set; } = default!;
        public string DefaultPassword { get; set; } = default!;
    }

}

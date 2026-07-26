using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Auth.Dtos
{
    public class UserListResponseDto
    {
        public string UserName { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string? Email { get; set; }
        public DateTime? LastLogin { get; set; }
    }

}

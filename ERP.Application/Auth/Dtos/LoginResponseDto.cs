using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Auth.Dtos
{
    public class LoginResponseDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = null!;
        public List<SystemInfoDto> Systems { get; set; } = new();
    }

    public class SystemInfoDto
    {
        public string SystemCode { get; set; } = null!;
        public string SystemName { get; set; } = null!;
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Auth.Interfaces.Security
{
    public interface IJwtTokenService
    {
        string GenerateToken(
            Guid userId,
            string systemCode,
            string permissionMask);
    }
}

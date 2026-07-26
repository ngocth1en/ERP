using ERP.Application.Auth.Interfaces.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Security
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(
            Guid userId,
            string systemCode,List<string> listSystem,
            string userName)
        {
            var claims = new[]
            {
            new Claim("uid", userId.ToString()),
            new Claim("system", systemCode),
            new Claim("systems", JsonSerializer.Serialize(listSystem)),
            new Claim("username",userName)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]!));

            var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

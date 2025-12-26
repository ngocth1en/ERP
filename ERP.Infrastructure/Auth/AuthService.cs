using ERP.Application.Auth.Dtos;
using ERP.Application.Auth.Interfaces;
using ERP.Application.Auth.Interfaces.Security;
using ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Auth
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher _passwordHasher;
        private readonly JwtTokenService _jwtTokenGenerator;

        public AuthService(
            AppDbContext db,
            IPasswordHasher passwordHasher,
            JwtTokenService jwtTokenGenerator)
        {
            _db = db;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            // 1. Verify user
            var user = await _db.Users
                .FirstOrDefaultAsync(x =>
                    x.UserName == request.Username &&
                    x.IsActive);

            if (user == null)
                throw new Exception("User không tồn tại hoặc đã bị khóa");

            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
                throw new Exception("Sai mật khẩu");

            // 2. Load systems user có quyền
            var systems = await _db.UserSystemPermissions
                .Where(x =>
                    x.UserId == user.Id &&
                    x.SystemModule.IsActive)
                .Select(x => new SystemInfoDto
                {
                    SystemCode = x.SystemCode,
                    SystemName = x.SystemModule.Name
                })
                .Distinct()
                .ToListAsync();

            if (!systems.Any())
                throw new Exception("User chưa được phân quyền hệ thống");

            return new LoginResponseDto
            {
                UserId = user.Id,
                Username = user.UserName,
                Systems = systems
            };
        }


        public async Task<string> SelectSystemAsync(Guid userId, string systemCode)
        {
            // 1. Verify permission
            var permission = await _db.UserSystemPermissions
                .Where(x =>
                    x.UserId == userId &&
                    x.SystemCode == systemCode &&
                    x.SystemModule.IsActive)
                .Select(x => x.Permission)
                .FirstOrDefaultAsync();

            if (permission == null)
                throw new Exception("Không có quyền truy cập hệ thống");

            // 2. Generate JWT
            return _jwtTokenGenerator.GenerateToken(
                userId,
                systemCode,
                permission
            );
        }

    }

}

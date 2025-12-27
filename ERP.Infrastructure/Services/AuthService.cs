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
        private readonly AppDbContext _context;
        private readonly JwtTokenService _jwtService;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(
            AppDbContext context,
            JwtTokenService jwtService,
            IPasswordHasher passwordHasher)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserName == request.UserName)
                ?? throw new UnauthorizedAccessException();

            if (!_passwordHasher.Verify(user.PasswordHash, request.Password))
                throw new UnauthorizedAccessException();

            var userSystem = await _context.UserSystems
                .FirstOrDefaultAsync(x =>
                    x.UserId == user.UserName &&
                    x.SystemCode == request.SystemCode &&
                    x.IsAllowed)
                ?? throw new UnauthorizedAccessException("No system permission");

            var token = _jwtService.GenerateToken(
                user.Id,
                request.SystemCode,
                userSystem.PermissionMask
            );

            return new LoginResponseDto
            {
                AccessToken = token
            };
        }
    }

}

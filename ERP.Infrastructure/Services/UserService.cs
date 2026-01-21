using ERP.Application.Auth.Dtos;
using ERP.Application.Auth.Interfaces;
using ERP.Application.Auth.Interfaces.Security;
using ERP.Domain.Entities;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMailService _mailService;

        public UserService(AppDbContext context, IPasswordHasher passwordHasher, IMailService mailService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _mailService = mailService;
        }

        public async Task<UserCreatedResponseDto> CreateAsync(CreateUserRequestDto request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var lastCode = await _context.Users
                .Where(x => x.UserName != "999999")
                .OrderByDescending(x => x.UserName)
                .Select(x => x.UserName)
                .FirstOrDefaultAsync();

            var userName = string.IsNullOrEmpty(lastCode)
                ? "000001"
                : (int.Parse(lastCode) + 1).ToString("D6");

            var user = new ERP.Domain.Entities.Users
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                DepartmentId = request.DepartmentId,

                FName = request.FName,
                LName = request.LName,
                Email = request.Email,
                Address = request.Address,
                PhoneNumber = request.PhoneNumber,
                BankName = request.BankName,
                BankNum = request.BankNum,

                MustChangePassword = true
            };

            user.PasswordHash = _passwordHasher.Hash(userName);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new UserCreatedResponseDto
            {
                UserName = userName,
                DefaultPassword = userName
            };
        }
        public async Task ChangePasswordAsync(string username, ChangePasswordRequestDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == username)
                ?? throw new Exception("User không tồn tại");

            if (!_passwordHasher.Verify(request.OldPassword, user.PasswordHash))
                throw new Exception("Mật khẩu cũ không đúng");

            user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
            user.MustChangePassword = false;

            await _context.SaveChangesAsync();
        }
        public async Task ForgotPasswordByEmailAsync(ForgotPasswordRequestDto request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == request.Email)
                ?? throw new Exception("Email không tồn tại trong hệ thống");

            // Reset mật khẩu = mã NV
            user.PasswordHash = _passwordHasher.Hash(user.UserName);
            user.MustChangePassword = true;

            await _context.SaveChangesAsync();

            // Gửi mail thông báo
            await _mailService.SendAsync(
                user.Email,
                "Reset mật khẩu thành công",
                $@"
Xin chào {user.LName} {user.FName},

Mật khẩu của bạn đã được reset thành công.

• Mã nhân viên: {user.UserName}
• Mật khẩu tạm thời: Mã nhân viên của bạn

Vui lòng đăng nhập và đổi mật khẩu ngay để đảm bảo an toàn.

Trân trọng,
Phòng IT
");
        }
        public async Task<List<UserListResponseDto>> GetUsersAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .OrderBy(x => x.UserName)
                .Select(x => new UserListResponseDto
                {
                    UserName = x.UserName,
                    FullName = (x.LName ?? "") + " " + (x.FName ?? ""),
                    Email = x.Email,
                    LastLogin = x.LastLogin
                })
                .ToListAsync();
        }

        public async Task<List<UserListResponseDto>> GetUsersByUserNameAsync(string username)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(x => x.UserName.StartsWith(username))
                .OrderBy(x => x.UserName)
                .Select(x => new UserListResponseDto
                {
                    UserName = x.UserName,
                    FullName = (x.LName ?? "") + " " + (x.FName ?? ""),
                    Email = x.Email,
                    LastLogin = x.LastLogin
                })
                .ToListAsync();
        }
        

    }

}

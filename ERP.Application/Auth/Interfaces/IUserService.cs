using ERP.Application.Auth.Dtos;
using ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Auth.Interfaces
{
    public interface IUserService
    {
        Task<UserCreatedResponseDto> CreateAsync(CreateUserRequestDto request);
        Task ChangePasswordAsync(string username, ChangePasswordRequestDto request);
        Task ForgotPasswordByEmailAsync(ForgotPasswordRequestDto request);
        Task<List<UserListResponseDto>> GetUsersAsync();
    }
}

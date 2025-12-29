using ERP.Application.Auth.Dtos;
using ERP.Application.Auth.Interfaces;
using ERP.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize] // chỉ admin gọi
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserRequestDto request)
            => Ok(await _userService.CreateAsync(request));
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(
    string userName,
    ChangePasswordRequestDto request)
        {
            await _userService.ChangePasswordAsync(userName, request);
            return Ok("Đổi mật khẩu thành công");
        }
        [AllowAnonymous]
        [HttpPost("forgot-password/email")]
        public async Task<IActionResult> ForgotPasswordByEmail(
    ForgotPasswordRequestDto request)
        {
            await _userService.ForgotPasswordByEmailAsync(request);
            return Ok("Mật khẩu đã được reset. Vui lòng kiểm tra email.");
        }

    }

}

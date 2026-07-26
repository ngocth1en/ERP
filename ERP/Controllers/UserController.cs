using ERP.Application.Auth.Dtos;
using ERP.Application.Auth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        private const string ADMIN_USERNAME = "999999";

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // =======================
        // CHỈ USERNAME = 999999
        // =======================
        [Authorize]
        [HttpPost("create-user")]
        public async Task<IActionResult> Create(CreateUserRequestDto request)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);

            if (userName != ADMIN_USERNAME)
            {
                return Forbid("Bạn không có quyền tạo user");
            }

            return Ok(await _userService.CreateAsync(request));
        }

        // =======================
        // USER ĐÃ LOGIN
        // =======================
        [AllowAnonymous]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(
            [FromQuery] string userName,
            [FromBody] ChangePasswordRequestDto request)
        {
            await _userService.ChangePasswordAsync(userName, request);
            return Ok("Đổi mật khẩu thành công");
        }

        // =======================
        // ANONYMOUS
        // =======================
        [AllowAnonymous]
        [HttpPost("forgot-password/email")]
        public async Task<IActionResult> ForgotPasswordByEmail(
            ForgotPasswordRequestDto request)
        {
            await _userService.ForgotPasswordByEmailAsync(request);
            return Ok("Mật khẩu đã được reset. Vui lòng kiểm tra email.");
        }

        // =======================
        // USER ĐÃ LOGIN
        // =======================
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _userService.GetUsersAsync();
            return Ok(result);
        }
        [AllowAnonymous]
        [HttpGet("search")] 
        public async Task<IActionResult> GetUserByUserName([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username không được để trống");
            }
            var result = await _userService.GetUsersByUserNameAsync(username);
            return Ok(result);
        }
    }
}

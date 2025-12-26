using ERP.Application.Auth.Dtos;
using ERP.Application.Auth.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto dto)
            => Ok(await _authService.LoginAsync(dto));

        [HttpPost("select-system")]
        public async Task<IActionResult> SelectSystem(Guid userId, string systemCode)
            => Ok(await _authService.SelectSystemAsync(userId, systemCode));
    }

}

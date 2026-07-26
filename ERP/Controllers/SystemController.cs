using ERP.Application.Systems.Dtos.Menu;
using ERP.Application.Systems.Dtos.Permission;
using ERP.Application.Systems.Dtos.System;
using ERP.Application.Systems.Interfaces;
using ERP.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/systems")]
    //[Authorize]
    public class SystemsController : ControllerBase
    {
        private readonly ISystemService _service;

        public SystemsController(ISystemService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("by-user/{username}")]
        public async Task<IActionResult> GetByUser(string username)
            => Ok(await _service.GetSystemsByUserAsync(username));
        [HttpGet("by-user-not-allowed/{username}")]
        public async Task<IActionResult> GetByUserNotAllowed(string username)
            => Ok(await _service.GetSystemsByUserAsync(username));
        [HttpGet("modules")]
        public async Task<IActionResult> GetModules()
        {
            return Ok(await _service.GetAllInfoSystemAsync());
        }
        //[HttpPut("modules/{code}")]
        //    public async Task<IActionResult> UpdateModule(
        //string code,
        //[FromBody] UpdateSystemModuleDto dto)
        //    {
        //        await _service.UpdateAsync(code, dto);
        //        return Ok(new { message = "Cập nhật hệ thống thành công" });
        //    }
        [HttpPut("modules/{code}")]
        public async Task<IActionResult> UpdateModule(
    string code,
    [FromBody] UpdateSystemModuleDto dto)
        {
            var updated = await _service.UpdateAsync(code, dto);
            return Ok(updated);
        }

        [HttpGet("{systemCode}/files")]
        public async Task<IActionResult> GetFiles(string systemCode)
       => Ok(await _service.GetGuideFilesAsync(systemCode));
        [HttpPost("{systemCode}/upload")]
        public async Task<IActionResult> Upload(
    string systemCode,
    [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ");

            var dto = new UploadSystemGuideFileDto
            {
                SystemCode = systemCode,
                FileName = file.FileName,
                FileStream = file.OpenReadStream()
            };

            await _service.UploadGuideFileAsync(dto);
            return Ok();
        }
        /// <summary>
        /// Cấp hệ thống cho user
        /// </summary>
        [HttpPost("grant")]
        public async Task<IActionResult> Grant([FromBody] GrantUserSystemDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserName) ||
                string.IsNullOrWhiteSpace(dto.SystemCode))
            {
                return BadRequest("UserName hoặc SystemCode không hợp lệ");
            }

            await _service.GrantAsync(dto);
            return Ok();
        }

        /// <summary>
        /// Thu hồi hệ thống khỏi user
        /// </summary>
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] GrantUserSystemDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserName) ||
                string.IsNullOrWhiteSpace(dto.SystemCode))
            {
                return BadRequest("UserName hoặc SystemCode không hợp lệ");
            }

            await _service.RevokeAsync(dto);
            return Ok();
        }
    }



}

using ERP.Application.Systems.Dtos;
using ERP.Application.Systems.Interfaces;
using ERP.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/systems")]
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
    }



}

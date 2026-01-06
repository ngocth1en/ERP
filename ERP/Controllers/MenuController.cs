using ERP.Application.Systems.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/menus")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        /// <summary>
        /// Lấy sidebar hệ thống ASSET
        /// </summary>
        [HttpGet("{systemCode}/sidebar")]
        public async Task<IActionResult> GetSidebar(string systemCode)
        {
            var result = await _menuService.GetSidebarBySystem(systemCode);
            return Ok(result);
        }
    }

}

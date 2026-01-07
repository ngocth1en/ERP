using ERP.Application.Systems.Dtos;
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
        [HttpGet("{systemCode}/sidebar/{menuCode}/permission")]
        public async Task<IActionResult> GetPermissionOnMenuByUser(
    string systemCode,
    string menuCode)
        {
            var userName = User.Identity!.Name!;

            var result = await _menuService
                .GetPermissionUserOnMenu(menuCode, userName);

            return Ok(result);
        }
        [HttpPost("permission")]
        public async Task<IActionResult> Create(
        [FromBody] CreateUserMenuPermissionDto dto)
        {
            await _menuService.CreateAsync(dto);
            return Ok("Thêm quyền thành công");
        }
        [HttpPost("permission/bulk")]
        public async Task<IActionResult> BulkAssignPermission(
    [FromBody] BulkUserMenuPermissionDto dto)
        {
            await _menuService.BulkUpsertUserMenuPermissionAsync(dto);
            return Ok("Gán quyền thành công");
        }
        /// <summary>
        /// Thêm menu mới
        /// </summary>
        [HttpPost("system-menu")]
        public async Task<IActionResult> CreateSystemMenu(
            [FromBody] CreateSystemMenuDto dto)
        {
            await _menuService.CreateMenuAsync(dto);
            return Ok("Thêm menu thành công");
        }

    }

}

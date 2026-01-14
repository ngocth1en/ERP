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
        public async Task<IActionResult> GetSidebar(string systemCode,string userName)
        {
            var result = await _menuService.GetSidebarBySystem(systemCode,userName);
            return Ok(result);
        }
        [HttpGet("{systemCode}/sidebar/{menuCode}/permission")]
        public async Task<IActionResult> GetPermissionOnMenuByUser(
    string systemCode,
    string menuCode,string userName)
        {

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
        [HttpPut("permission/bulk")]
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
        [HttpGet("{systemCode}/status")]
        public async Task<IActionResult> GetMenuStatus(string systemCode)
        {
            var result = await _menuService.GetMenuStatusAsync(systemCode);
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật trạng thái menu (đang dùng / ngừng dùng)
        /// </summary>
        [HttpPut("status")]
        public async Task<IActionResult> UpdateMenuStatus(
            [FromBody] UpdateSystemMenuStatusDto dto)
        {
            await _menuService.UpdateMenuStatusAsync(dto);
            return Ok("Cập nhật trạng thái menu thành công");
        }
        [HttpPut]
        public async Task<IActionResult> UpdatePermission(
        [FromBody] UpdateUserMenuPermissionDto dto)
        {
            await _menuService.UpsertAsync(dto);
            return Ok();
        }
        [HttpGet("menu-tree")]
        public async Task<IActionResult> GetMenuTreeBySystem([FromQuery] string systemCode)
        {
            var data = await _menuService.GetMenuTreeBySystem(systemCode);
            return Ok(data);
        }
        [HttpPost("permissions/system")]
        public async Task<IActionResult> GetAllPermissionUserOnSystem(
    [FromBody] UserMenuPermissionRequestDto req)
        {
            var result = await _menuService.GetAllPermissionUserOnSystem(req);
            return Ok(result);
        }
    }

}

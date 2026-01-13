using ERP.Application.Systems.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Systems.Interfaces
{
    public interface IMenuService
    {
        Task<List<SidebarItemDto>> GetSidebarBySystem(string sysCode,string userName);
        Task<string> GetPermissionUserOnMenu(string menuCode,string userName);
        Task CreateAsync(CreateUserMenuPermissionDto dto);
        Task BulkUpsertUserMenuPermissionAsync(BulkUserMenuPermissionDto dto);
        Task CreateMenuAsync(CreateSystemMenuDto dto);
        Task<List<SystemMenuStatusDto>> GetMenuStatusAsync(string systemCode);
        Task UpdateMenuStatusAsync(UpdateSystemMenuStatusDto dto);
        Task<List<MenuParentDto>> GetMenuParentsAsync(
        string systemCode);
        Task<List<MenuChildDto>> GetMenuChildrenAsync(
        string parentCode);
        Task CreateParentMenuAsync(
        CreateSystemMenuDto dto);
        Task CreateChildMenuAsync(
        CreateSystemMenuDto dto);
        Task<List<SidebarMenuManageDto>> GetMenuTreeBySystem(string systemCode);
    }

}

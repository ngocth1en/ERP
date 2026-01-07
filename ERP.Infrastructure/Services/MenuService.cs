using ERP.Application.Systems.Dtos;
using ERP.Application.Systems.Interfaces;
using ERP.Domain.Entities;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Services
{
    public class MenuService : IMenuService
    {
        private readonly AppDbContext _context;

        public MenuService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SidebarItemDto>> GetSidebarBySystem(string sysCode)
        {
            var menus = await _context.SystemMenus
                .Where(x => x.SystemCode == sysCode)
                .OrderBy(x => x.GroupCode)
                .ThenBy(x => x.OrderNo)
                .ToListAsync();

            var groups = menus
                .GroupBy(x => x.GroupCode)
                .Select(g => new SidebarItemDto
                {
                    Key = g.Key,
                    Label = GetGroupLabel(g.Key),
                    Children = g.Select(m => new SidebarItemDto
                    {
                        Key = m.Code,
                        Label = m.Name,
                        Path = m.Route,
                        Permission = m.Code
                    }).ToList()
                })
                .ToList();

            return groups;
        }

        //private static string GetGroupLabel(string groupCode)
        //{
        //    return groupCode switch
        //    {
        //        "ASSET_MASTER" => "Tài sản cố định",
        //        "ASSET_TRANSACTION" => "Chức năng giao dịch",
        //        "ASSET_REPORT" => "Báo biểu",
        //        _ => groupCode
        //    };
        //}
        private static string GetGroupLabel(string groupCode)
        {
            if (groupCode.StartsWith("ASSET_"))
            {
                return groupCode switch
                {
                    "ASSET_MASTER" => "Danh mục tài sản",
                    "ASSET_TRANSACTION" => "Nghiệp vụ tài sản",
                    "ASSET_REPORT" => "Báo cáo tài sản",
                    _ => "Tài sản"
                };
            }

            if (groupCode.StartsWith("MAINT_"))
            {
                return groupCode switch
                {
                    "MAINT_ACCOUNT" => "Tài khoản người dùng",
                    "MAINT_SYSTEM" => "Quản lý hệ thống",
                    "MAINT_LANGUAGE" => "Ngôn ngữ hiển thị",
                    "MAINT_PERMISSION" => "Phân quyền",
                    _ => "Hệ thống"
                };
            }

            return groupCode;
        }

        public async Task<string> GetPermissionUserOnMenu(string menuCode, string userName)
        {
            // 1. Lấy UserId theo UserName
            var userId = await _context.Users
                .Where(u => u.UserName == userName)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            // 2. Lấy permission theo UserId + SysCode
            var permission = await _context.UserMenuPermissions
                .Where(p => p.UserId == userId && p.MenuCode == menuCode)
                .FirstOrDefaultAsync();


            return permission.ToString();
        }
        public async Task CreateAsync(CreateUserMenuPermissionDto dto)
        {
            // 1. Check User tồn tại
            var userExists = await _context.Users
                .AnyAsync(u => u.UserName == dto.UserName);

            if (!userExists)
                throw new Exception("User không tồn tại");

            // 2. Check Menu tồn tại
            var menuExists = await _context.SystemMenus
                .AnyAsync(m => m.Code == dto.MenuCode);

            if (!menuExists)
                throw new Exception("Menu không tồn tại");
            var userId = await _context.Users
                .Where(u => u.UserName == dto.UserName)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();
            // 3. Check đã có quyền chưa
            var existed = await _context.UserMenuPermissions
                .AnyAsync(p => p.UserId == userId
                            && p.MenuCode == dto.MenuCode);

            if (existed)
                throw new Exception("User đã có quyền trên menu này");

            // 4. Insert
            var permission = new UserMenuPermission
            {
                UserId = userId,
                MenuCode = dto.MenuCode,
                PermissionMask = dto.PermissionMask
            };

            _context.UserMenuPermissions.Add(permission);
            await _context.SaveChangesAsync();
        }
        public async Task BulkUpsertUserMenuPermissionAsync(
    BulkUserMenuPermissionDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Lấy toàn bộ menu thuộc system
                var systemMenus = await _context.SystemMenus
                    .Where(m => m.SystemCode == dto.SystemCode)
                    .Select(m => m.Code)
                    .ToListAsync();

                // 2. Chỉ xử lý menu hợp lệ
                var validMenuCodes = dto.Menus
                    .Where(m => systemMenus.Contains(m.MenuCode))
                    .ToList();

                // 3. Lấy permission hiện có
                var existedPermissions = await _context.UserMenuPermissions
                    .Where(p => p.UserId == dto.UserId
                             && validMenuCodes.Select(x => x.MenuCode)
                                .Contains(p.MenuCode))
                    .ToListAsync();

                foreach (var item in validMenuCodes)
                {
                    var permission = existedPermissions
                        .FirstOrDefault(p => p.MenuCode == item.MenuCode);

                    if (permission != null)
                    {
                        // UPDATE
                        permission.PermissionMask = item.PermissionMask;
                    }
                    else
                    {
                        // INSERT
                        _context.UserMenuPermissions.Add(new UserMenuPermission
                        {
                            UserId = dto.UserId,
                            MenuCode = item.MenuCode,
                            PermissionMask = item.PermissionMask
                        });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task CreateMenuAsync(CreateSystemMenuDto dto)
        {
            // 1. Check trùng Code
            var existed = await _context.SystemMenus
                .AnyAsync(m => m.Code == dto.Code);

            if (existed)
                throw new Exception("Menu Code đã tồn tại");

            // 2. Check System tồn tại
            var systemExists = await _context.SystemModules
                .AnyAsync(s => s.Code == dto.SystemCode);

            if (!systemExists)
                throw new Exception("SystemCode không tồn tại");

            // 3. Insert menu
            var menu = new SystemMenu
            {
                Code = dto.Code,
                SystemCode = dto.SystemCode,
                GroupCode = dto.GroupCode,
                Name = dto.Name,
                Route = dto.Route,
                OrderNo = dto.OrderNo
            };

            _context.SystemMenus.Add(menu);
            await _context.SaveChangesAsync();
        }

    }

}

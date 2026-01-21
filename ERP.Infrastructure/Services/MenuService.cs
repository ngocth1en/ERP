using ERP.Application.Common;
using ERP.Application.Systems.Dtos.Menu;
using ERP.Application.Systems.Dtos.Permission;
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
        public async Task<List<SidebarItemDto>> GetSidebarBySystem(
    string systemCode,
    string userName)
        {
            // 1️⃣ Lấy UserId
            var userId = await _context.Users
                .Where(u => u.UserName == userName)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            // 2️⃣ Lấy toàn bộ menu đang dùng của system
            var menus = await _context.SystemMenus
                .Where(m =>
                    m.SystemCode == systemCode &&
                    m.IsUsing)
                .OrderBy(m => m.OrderNo)
                .ToListAsync();

            // 3️⃣ Lấy permission của user
            var permissions = await _context.UserMenuPermissions
                .Where(p => p.UserId == userId)
                .ToDictionaryAsync(
                    p => p.MenuCode,
                    p => p.PermissionMask
                );

            // 4️⃣ Lấy menu CHA
            var parentMenus = menus
                .Where(m => m.ParentCode == null)
                .OrderBy(m => m.OrderNo)
                .ToList();

            // 5️⃣ Build sidebar
            var sidebar = parentMenus
                .Select(parent => new SidebarItemDto
                {
                    Key = parent.Code,
                    Label = parent.Name,

                    Children = menus
                        .Where(child => child.ParentCode == parent.Code)
                        .OrderBy(child => child.OrderNo)
                        .Select(child => new SidebarItemDto
                        {
                            Key = child.Code,
                            Label = child.Name,
                            Path = child.Route,
                            Permission = permissions.TryGetValue(child.Code, out var mask)
                                ? mask
                                : "0000000000"
                        })
                        .ToList()
                })
                // ❗ chỉ hiển thị menu cha nếu có menu con
                .Where(x => x.Children.Any())
                .ToList();

            return sidebar;
        }
        public async Task<List<SidebarMenuManageDto>> GetMenuTreeBySystem(string systemCode)
        {
            // 1️⃣ Lấy toàn bộ menu của system (kể cả ngừng)
            var menus = await _context.SystemMenus
                .Where(m => m.SystemCode == systemCode)
                .OrderBy(m => m.OrderNo)
                .ToListAsync();

            // 2️⃣ Menu CHA
            var parentMenus = menus
                .Where(m => m.ParentCode == null)
                .OrderBy(m => m.OrderNo)
                .ToList();

            // 3️⃣ Build tree
            var result = parentMenus
                .Select(parent => new SidebarMenuManageDto
                {
                    Key = parent.Code,
                    Label = parent.Name,
                    IsUsing = parent.IsUsing,

                    Children = menus
                        .Where(child => child.ParentCode == parent.Code)
                        .OrderBy(child => child.OrderNo)
                        .Select(child => new SidebarMenuManageDto
                        {
                            Key = child.Code,
                            Label = child.Name,
                            IsUsing = child.IsUsing
                        })
                        .ToList()
                })
                .ToList();

            return result;
        }

        public async Task<PagedResult<UserMenuPermissionResponseDto>>
    GetAllPermissionUserOnSystem(UserMenuPermissionRequestDto req)
        {
            // 1. Lấy UserId
            var userId = await _context.Users
                .Where(u => u.UserName == req.UserName)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (userId == Guid.Empty)
                throw new Exception("User not found");

            // 2. Query menu + left join permission
            var query =
                from menu in _context.SystemMenus
                join perm in _context.UserMenuPermissions
                    on new { MenuCode = menu.Code, UserId = userId }
                    equals new { perm.MenuCode, perm.UserId }
                    into mp
                from permission in mp.DefaultIfEmpty()
                where menu.SystemCode == req.SystemCode
                      && menu.ParentCode != null
                      && menu.IsUsing
                orderby menu.OrderNo
                select new UserMenuPermissionResponseDto
                {
                    MenuCode = menu.Code,
                    MenuName = menu.Name,
                    PermissionMask = permission != null
                        ? permission.PermissionMask
                        : "0000000000"
                };

            // 3. Tổng số record
            var total = await query.CountAsync();

            // 4. Phân trang
            var items = await query
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync();

            return new PagedResult<UserMenuPermissionResponseDto>
            {
                Total = total,
                Items = items
            };
        }
        public async Task UpsertAsync(UpdateUserMenuPermissionDto dto)
        {
            // 1️⃣ Lấy UserId
            var userId = await _context.Users
                .Where(u => u.UserName == dto.UserName)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (userId == null)
                throw new Exception("User không tồn tại");

            // 2️⃣ Kiểm tra menu có thuộc system không
            var menuExists = await _context.SystemMenus.AnyAsync(m =>
                m.Code == dto.MenuCode &&
                m.SystemCode == dto.SystemCode);

            if (!menuExists)
                throw new Exception("Menu không thuộc hệ thống");

            // 3️⃣ Upsert permission
            var permission = await _context.UserMenuPermissions
                .FirstOrDefaultAsync(p =>
                    p.UserId == userId &&
                    p.MenuCode == dto.MenuCode);

            if (permission == null)
            {
                permission = new ERP.Domain.Entities.UserMenuPermissions
                {
                    UserId = userId,
                    MenuCode = dto.MenuCode,
                    PermissionMask = dto.PermissionMask,
                    CreatedAt = DateTime.Now
                };

                _context.UserMenuPermissions.Add(permission);
            }
            else
            {
                permission.PermissionMask = dto.PermissionMask;
                permission.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
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
            var permission = new ERP.Domain.Entities.UserMenuPermissions
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
                var userId = await _context.Users
                .Where(u => u.UserName == dto.UserName)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();
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
                    .Where(p => p.UserId == userId
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
                        permission.UpdatedAt = DateTime.Now;
                    }
                    else
                    {
                        // INSERT
                        _context.UserMenuPermissions.Add(new ERP.Domain.Entities.UserMenuPermissions
                        {
                            UserId = userId,
                            MenuCode = item.MenuCode,
                            PermissionMask = item.PermissionMask,
                            CreatedAt=DateTime.Now
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
            // 1️⃣ Check trùng Code
            var existed = await _context.SystemMenus
                .AnyAsync(m => m.Code == dto.Code);

            if (existed)
                throw new Exception("Menu Code đã tồn tại");

            // 2️⃣ Check System tồn tại
            var systemExists = await _context.SystemModules
                .AnyAsync(s => s.Code == dto.SystemCode);

            if (!systemExists)
                throw new Exception("SystemCode không tồn tại");

            // 3️⃣ Nếu có ParentCode → check menu cha tồn tại
            if (!string.IsNullOrEmpty(dto.ParentCode))
            {
                var parentExists = await _context.SystemMenus
                    .AnyAsync(m =>
                        m.Code == dto.ParentCode &&
                        m.SystemCode == dto.SystemCode);

                if (!parentExists)
                    throw new Exception("Parent menu không tồn tại");
            }

            // 4️⃣ Insert menu
            var menu = new ERP.Domain.Entities.SystemMenus
            {
                Code = dto.Code,
                SystemCode = dto.SystemCode,

                ParentCode = dto.ParentCode, // 🔥 menu cha – con
                Name = dto.Name,
                Route = dto.Route,
                OrderNo = dto.OrderNo,

                IsUsing = true // ✅ mặc định đang sử dụng
            };

            _context.SystemMenus.Add(menu);
            await _context.SaveChangesAsync();
        }

        public async Task<List<SystemMenuStatusDto>> GetMenuStatusAsync(string systemCode)
        {
            return await _context.SystemMenus
                .AsNoTracking()
                .Where(x => x.SystemCode == systemCode)
                .OrderBy(x => x.OrderNo)
                .Select(x => new SystemMenuStatusDto
                {
                    Code = x.Code,
                    Name = x.Name,
                    IsUsing = x.IsUsing
                })
                .ToListAsync();
        }

        public async Task UpdateMenuStatusAsync(UpdateSystemMenuStatusDto dto)
        {
            var menu = await _context.SystemMenus
                .FirstOrDefaultAsync(x => x.Code == dto.Code)
                ?? throw new Exception("Menu không tồn tại");
            if(!String.IsNullOrEmpty(dto.Name))
                menu.Name = dto.Name;
            menu.IsUsing = dto.IsUsing;
            await _context.SaveChangesAsync();
        }
        public async Task<List<MenuParentDto>> GetMenuParentsAsync(string systemCode)
        {
            return await _context.SystemMenus
                .AsNoTracking()
                .Where(x =>
                    x.SystemCode == systemCode &&
                    x.ParentCode == null)
                .OrderBy(x => x.OrderNo)
                .Select(x => new MenuParentDto
                {
                    Code = x.Code,
                    Name = x.Name
                })
                .ToListAsync();
        }
        public async Task<List<MenuChildDto>> GetMenuChildrenAsync(string parentCode)
        {
            return await _context.SystemMenus
                .AsNoTracking()
                .Where(x => x.ParentCode == parentCode)
                .OrderBy(x => x.OrderNo)
                .Select(x => new MenuChildDto
                {
                    Code = x.Code,
                    Name = x.Name,
                    IsUsing = x.IsUsing
                })
                .ToListAsync();
        }
        public async Task CreateParentMenuAsync(CreateSystemMenuDto dto)
        {
            dto.ParentCode = null;
            await CreateMenuAsync(dto);
        }
        public async Task CreateChildMenuAsync(CreateSystemMenuDto dto)
        {
            if (string.IsNullOrEmpty(dto.ParentCode))
                throw new Exception("Menu con bắt buộc phải có ParentCode");

            await CreateMenuAsync(dto);
        }

        
    }

}

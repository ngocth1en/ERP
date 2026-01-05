using ERP.Application.Systems.Dtos;
using ERP.Application.Systems.Interfaces;
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

        private static string GetGroupLabel(string groupCode)
        {
            return groupCode switch
            {
                "ASSET_MASTER" => "Tài sản cố định",
                "ASSET_TRANSACTION" => "Chức năng giao dịch",
                "ASSET_REPORT" => "Báo biểu",
                _ => groupCode
            };
        }
    }

}

using ERP.Application.Auth.Interfaces.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly AppDbContext _context;

        public PermissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SystemModule>> GetSystemsByUserAsync(Guid userId)
        {
            return await _context.UserSystemPermissions
                .Where(x => x.UserId == userId)
                .Select(x => x.SystemModule)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<UserSystemPermission?> GetPermissionAsync(
            Guid userId,
            string systemCode)
        {
            return await _context.UserSystemPermissions
                .Include(x => x.SystemModule)
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.SystemModule.Code == systemCode);
        }
    }
}

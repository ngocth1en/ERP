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
    public class SystemService : ISystemService
    {
        private readonly AppDbContext _context;

        public SystemService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SystemOptionDto>> GetAllAsync()
        {
            return await _context.SystemModules
                .Select(x => new SystemOptionDto
                {
                    Code = x.Code,
                    Name = x.Name
                })
                .ToListAsync();
        }

        public async Task<List<SystemOptionDto>> GetSystemsByUserAsync(string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserName == username);

            if (user == null) return new();

            return await _context.UserSystems
                .Where(x => x.UserId == user.Id && x.IsAllowed)
                .Select(x => new SystemOptionDto
                {
                    Code = x.SystemCode,
                    Name = x.SystemModule.Name
                })
                .ToListAsync();
        }
    }
}

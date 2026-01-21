using ERP.Application.Systems.Dtos.Menu;
using ERP.Application.Systems.Dtos.Permission;
using ERP.Application.Systems.Dtos.System;
using ERP.Application.Systems.Interfaces;
using ERP.Infrastructure.ERP.Infrastructure.Persistence;
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

        public async Task<List<SystemModuleDto>> GetAllInfoSystemAsync()
        {
            return await _context.SystemModules
            .OrderBy(x => x.Name)
            .Select(x => new SystemModuleDto
            {
                Code = x.Code,
                Name = x.Name,
                IsActive = x.IsActive,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync();
        }

        public async Task<List<SystemGuideFileDto>> GetGuideFilesAsync(string systemCode)
        {
            return await _context.SystemGuideFiles
                .Where(x => x.SystemCode == systemCode)
                .OrderByDescending(x => x.UploadedAt)
                .Select(x => new SystemGuideFileDto
                {
                    Id = x.Id,
                    FileName = x.FileName,
                    UploadedAt = x.UploadedAt
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
                    Name = x.SystemCodeNavigation.Name
                })
                .ToListAsync();
        }

        public async Task<List<SystemOptionDto>> GetSystemsByUserNotAllowedAsync(string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserName == username);

            if (user == null) return new();

            return await _context.UserSystems
                .Where(x => x.UserId == user.Id && x.IsAllowed==false)
                .Select(x => new SystemOptionDto
                {
                    Code = x.SystemCode,
                    Name = x.SystemCodeNavigation.Name
                })
                .ToListAsync();
        }
        public async Task GrantAsync(GrantUserSystemDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserName == dto.UserName);

            if (user == null)
                throw new Exception($"User {dto.UserName} không tồn tại");

            var entity = await _context.UserSystems
                .FirstOrDefaultAsync(x =>
                    x.UserId == user.Id &&
                    x.SystemCode == dto.SystemCode);

            // Đã tồn tại → bật lại quyền
            if (entity != null)
            {
                if (!entity.IsAllowed)
                {
                    entity.IsAllowed = true;
                    await _context.SaveChangesAsync();
                }
                return;
            }

            // Chưa tồn tại → tạo mới
            _context.UserSystems.Add(new ERP.Domain.Entities.UserSystems
            {
                UserId = user.Id,
                SystemCode = dto.SystemCode,
                IsAllowed = true
            });

            await _context.SaveChangesAsync();
        }


        /* ======================
           REVOKE
        ====================== */
        public async Task RevokeAsync(GrantUserSystemDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserName == dto.UserName);

            if (user == null)
                throw new Exception($"User {dto.UserName} không tồn tại");

            var entity = await _context.UserSystems
                .FirstOrDefaultAsync(x =>
                    x.UserId == user.Id &&
                    x.SystemCode == dto.SystemCode);

            if (entity == null)
                return;

            entity.IsAllowed = false;
            await _context.SaveChangesAsync();
        }

        //public async Task UpdateAsync(string code, UpdateSystemModuleDto dto)
        //{
        //    var system = await _context.SystemModules
        //        .FirstOrDefaultAsync(x => x.Code == code);

        //    if (system == null)
        //        throw new Exception("System not found");

        //    system.Name = dto.Name;
        //    system.IsActive = dto.IsActive;
        //    system.UpdatedAt = DateTime.UtcNow;

        //    await _context.SaveChangesAsync();
        //}
        public async Task<SystemModuleDto> UpdateAsync(string code, UpdateSystemModuleDto dto)
        {
            var system = await _context.SystemModules
                .FirstOrDefaultAsync(x => x.Code == code);

            if (system == null)
                throw new Exception("System not found");

            system.Name = dto.Name;
            system.IsActive = dto.IsActive;
            system.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new SystemModuleDto
            {
                Code = system.Code,
                Name = system.Name,
                IsActive = system.IsActive,
                UpdatedAt = system.UpdatedAt
            };
        }


        public async Task UploadGuideFileAsync(UploadSystemGuideFileDto dto)
        {
            var folder = Path.Combine("uploads", "system-guides", dto.SystemCode);
            Directory.CreateDirectory(folder);

            var filePath = Path.Combine(folder, dto.FileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await dto.FileStream.CopyToAsync(stream);

            _context.SystemGuideFiles.Add(new ERP.Domain.Entities.SystemGuideFiles
            {
                SystemCode = dto.SystemCode,
                FileName = dto.FileName,
                FilePath = filePath,
                UploadedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }

    }
}

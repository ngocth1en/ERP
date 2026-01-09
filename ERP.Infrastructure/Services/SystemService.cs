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
                    Name = x.SystemModule.Name
                })
                .ToListAsync();
        }

        public async Task UpdateAsync(string code, UpdateSystemModuleDto dto)
        {
            var system = await _context.SystemModules
                .FirstOrDefaultAsync(x => x.Code == code);

            if (system == null)
                throw new Exception("System not found");

            system.Name = dto.Name;
            system.IsActive = dto.IsActive;
            system.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task UploadGuideFileAsync(UploadSystemGuideFileDto dto)
        {
            var folder = Path.Combine("uploads", "system-guides", dto.SystemCode);
            Directory.CreateDirectory(folder);

            var filePath = Path.Combine(folder, dto.FileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await dto.FileStream.CopyToAsync(stream);

            _context.SystemGuideFiles.Add(new SystemGuideFile
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

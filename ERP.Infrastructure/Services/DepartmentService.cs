using ERP.Application.Department.Dtos;
using ERP.Application.Department.Interfaces;
using ERP.Infrastructure.ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "departments_options";

        public DepartmentService(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<DepartmentOptionDto>> GetOptionsAsync()
        {
            // ✅ Dữ liệu đơn vị ít thay đổi, được đọc rất nhiều nơi (form, filter...)
            // → cache 30 phút trong RAM của chính process để giảm tải DB.
            var cached = await _cache.GetOrCreateAsync(CacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

                return await _context.Departments
                    .AsNoTracking()
                    .OrderBy(x => x.Name)
                    .Select(x => new DepartmentOptionDto
                    {
                        Id = x.Id,
                        Name = x.Name
                    })
                    .ToListAsync();
            });

            return cached ?? new List<DepartmentOptionDto>();
        }

        // ✅ Gọi hàm này sau khi Create/Update/Delete Department (nếu có thao tác ghi dữ liệu)
        public void InvalidateCache()
        {
            _cache.Remove(CacheKey);
        }
    }
}
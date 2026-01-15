using ERP.Application.Auth.Interfaces;
using ERP.Application.Common;
using ERP.Application.StorageLocation.Dtos.StorageLocation;
using ERP.Application.StorageLocation.Interfaces;
using ERP.Domain.Entities.ASM;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Services
{
    public class StorageLocationService : IStorageLocationService
    {
        private readonly AppDbContext _context;
        private readonly ITranslationService _translationService;

        public StorageLocationService(
            AppDbContext context,
            ITranslationService translationService)
        {
            _context = context;
            _translationService = translationService;
        }

        // =========================
        // LẤY TẤT CẢ VỊ TRÍ
        // =========================
        public async Task<List<StorageLocationDto>> GetAllAsync(string languageCode)
        {
            // 1️⃣ Load vị trí
            var locations = await _context.StorageLocations
                .AsNoTracking()
                .ToListAsync();

            // 2️⃣ Load translation 1 lần
            var translations = await _translationService
                .GetByLanguageAsync(languageCode);

            // 3️⃣ Map DTO
            return locations.Select(x =>
            {
                var key = $"STORAGE_LOCATION_STATUS_{x.Status}";

                return new StorageLocationDto
                {
                    StorageLocationNo = x.StorageLocationNo,
                    NameEN = x.NameEN,
                    NameVN = x.NameVN,
                    Status = x.Status,
                    InUse = x.inUse,
                    StatusText = translations.TryGetValue(key, out var value)
                        ? value
                        : key
                };
            }).ToList();
        }

        // =========================
        // LẤY THEO ID
        // =========================
        public async Task<StorageLocationDto?> GetByIdAsync(
            string storageLocationNo,
            string languageCode)
        {
            var entity = await _context.StorageLocations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.StorageLocationNo == storageLocationNo);

            if (entity == null) return null;

            return new StorageLocationDto
            {
                StorageLocationNo = entity.StorageLocationNo,
                NameEN = entity.NameEN,
                NameVN = entity.NameVN,
                Status = entity.Status,
                InUse = entity.inUse,
                StatusText = await _translationService.GetValueAsync(
                    $"STORAGE_LOCATION_STATUS_{entity.Status}",
                    languageCode)
                    ?? $"STORAGE_LOCATION_STATUS_{entity.Status}"
            };
        }

        // =========================
        // CẬP NHẬT TRẠNG THÁI
        // =========================
        public async Task UpdateStatusAsync(
            string storageLocationNo,
            int status)
        {
            var entity = await _context.StorageLocations
                .Include(x => x.Assets)
                .FirstOrDefaultAsync(x => x.StorageLocationNo == storageLocationNo);

            if (entity == null)
                throw new Exception("Storage location not found");

            // Rule nghiệp vụ
            if (status == 3 && entity.Assets.Any())
                throw new Exception("Không thể báo phế khi còn tài sản");

            entity.Status = status;
            await _context.SaveChangesAsync();
        }
        private static string GetPrefix(int status)
        {
            return status switch
            {
                1 => "U",
                2 => "N",
                3 => "S",
                _ => throw new Exception("Status không hợp lệ")
            };
        }
        private async Task<string> GenerateCodeAsync(int status, int floor)
        {
            var prefix = GetPrefix(status);
            var pattern = $"{prefix}{floor}F";

            var maxCode = await _context.StorageLocations
                .Where(x => x.StorageLocationNo.StartsWith(pattern))
                .OrderByDescending(x => x.StorageLocationNo)
                .Select(x => x.StorageLocationNo)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (maxCode != null)
            {
                var numberPart = maxCode.Substring(pattern.Length);
                nextNumber = int.Parse(numberPart) + 1;
            }

            return $"{pattern}{nextNumber:D3}";
        }
        public async Task CreateAsync(CreateStorageLocationDto dto)
        {
            var prefix = GetPrefix(dto.Status);
            var code = await GenerateCodeAsync(dto.Status, dto.Floor);

            var location = new StorageLocation
            {
                StorageLocationNo = code,
                NameVN = $"{dto.Floor}F_{prefix}_{dto.Name}",
                NameEN = dto.Name,
                DepartmentId = dto.DepartmentId,
                Status = dto.Status,
                inUse = dto.InUse
            };

            _context.StorageLocations.Add(location);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(string storageLocationNo, UpdateStorageLocationDto dto)
        {
            var entity = await _context.StorageLocations
                .FirstOrDefaultAsync(x => x.StorageLocationNo == storageLocationNo);

            if (entity == null)
                throw new Exception("Không tìm thấy vị trí");

            var prefix = GetPrefix(dto.Status);
            var floor = entity.StorageLocationNo.Substring(1, 1); // ví dụ U3F001 → 3

            entity.NameVN = $"{floor}F_{prefix}_{dto.Name}";
            entity.Status = dto.Status;
            entity.inUse = dto.InUse;

            await _context.SaveChangesAsync();
        }
        public async Task<PagedResult<StorageLocationListDto>> SearchAsync(
    SearchStorageLocationDto dto)
        {
            var query = _context.StorageLocations
                .Include(x => x.Department)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(dto.Name))
                query = query.Where(x => x.NameVN.Contains(dto.Name));

            if (!string.IsNullOrWhiteSpace(dto.DepartmentId))
                query = query.Where(x => x.DepartmentId == dto.DepartmentId);

            if (dto.Status.HasValue)
                query = query.Where(x => x.Status == dto.Status);

            if (dto.InUse.HasValue)
                query = query.Where(x => x.inUse == dto.InUse);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.StorageLocationNo) // mới lên đầu
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select((x, i) => new StorageLocationListDto
                {
                    STT = (dto.Page - 1) * dto.PageSize + i + 1,
                    StorageLocationNo = x.StorageLocationNo,
                    NameVN = x.NameVN,
                    DepartmentName = x.Department.Name,
                    Status = x.Status,
                    InUse = x.inUse
                })
                .ToListAsync();

            return new PagedResult<StorageLocationListDto>
            {
                Total = total,
                Items = items
            };
        }



    }
}

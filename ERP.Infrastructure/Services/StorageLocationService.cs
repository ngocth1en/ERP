using ERP.Application.Auth.Dtos;
using ERP.Application.Auth.Interfaces;
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
    }
}

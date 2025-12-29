using ERP.Application.Auth.Dtos;
using ERP.Application.Auth.Interfaces;
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
    public class TranslationService : ITranslationService
    {
        private readonly AppDbContext _context;

        public TranslationService(AppDbContext context)
        {
            _context = context;
        }

        // FE load toàn bộ text theo ngôn ngữ
        public async Task<Dictionary<string, string>> GetByLanguageAsync(string languageCode)
        {
            return await _context.Translations
                .Where(x => x.LanguageCode == languageCode)
                .ToDictionaryAsync(x => x.Key, x => x.Value);
        }

        // BE cần dịch 1 key
        public async Task<string?> GetValueAsync(string key, string languageCode)
        {
            return await _context.Translations
                .Where(x => x.Key == key && x.LanguageCode == languageCode)
                .Select(x => x.Value)
                .FirstOrDefaultAsync();
        }

        // Admin thêm hoặc sửa
        public async Task CreateOrUpdateAsync(TranslationUpsertDto dto)
        {
            var entity = await _context.Translations
                .FirstOrDefaultAsync(x =>
                    x.Key == dto.Key &&
                    x.LanguageCode == dto.LanguageCode);

            if (entity == null)
            {
                entity = new Translation
                {
                    Key = dto.Key,
                    LanguageCode = dto.LanguageCode,
                    Value = dto.Value
                };
                _context.Translations.Add(entity);
            }
            else
            {
                entity.Value = dto.Value;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string key, string languageCode)
        {
            var entity = await _context.Translations
                .FirstOrDefaultAsync(x =>
                    x.Key == key &&
                    x.LanguageCode == languageCode);

            if (entity == null)
                throw new Exception("Translation not found");

            _context.Translations.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

}

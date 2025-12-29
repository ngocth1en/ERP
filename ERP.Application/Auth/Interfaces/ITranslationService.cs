using ERP.Application.Auth.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Application.Auth.Interfaces
{
    public interface ITranslationService
    {
        Task<Dictionary<string, string>> GetByLanguageAsync(string languageCode);
        Task<string?> GetValueAsync(string key, string languageCode);

        Task CreateOrUpdateAsync(TranslationUpsertDto dto);
        Task DeleteAsync(string key, string languageCode);
    }

}

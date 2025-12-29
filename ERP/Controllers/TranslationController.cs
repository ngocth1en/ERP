using ERP.Application.Auth.Dtos;
using ERP.Application.Auth.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/translations")]
    public class TranslationController : ControllerBase
    {
        private readonly ITranslationService _service;

        public TranslationController(ITranslationService service)
        {
            _service = service;
        }

        // FE gọi khi load app
        // GET: api/translations/vi
        [HttpGet("{languageCode}")]
        public async Task<IActionResult> GetByLanguage(string languageCode)
        {
            var result = await _service.GetByLanguageAsync(languageCode);
            return Ok(result);
        }

        // BE dùng riêng
        // GET: api/translations/value?key=LOGIN&lang=vi
        [HttpGet("value")]
        public async Task<IActionResult> GetValue(string key, string lang)
        {
            var value = await _service.GetValueAsync(key, lang);
            return Ok(value ?? key); // fallback
        }

        // Admin thêm/sửa
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(TranslationUpsertDto dto)
        {
            await _service.CreateOrUpdateAsync(dto);
            return Ok("Saved");
        }

        // Admin xóa
        [HttpDelete]
        public async Task<IActionResult> Delete(string key, string lang)
        {
            await _service.DeleteAsync(key, lang);
            return Ok("Deleted");
        }
    }

}

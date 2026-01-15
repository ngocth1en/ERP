using ERP.Application.Auth.Interfaces;
using ERP.Application.Common;
using ERP.Application.StorageLocation.Dtos.StorageLocation;
using ERP.Application.StorageLocation.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/storage-locations")]
    public class StorageLocationsController : ControllerBase
    {
        private readonly IStorageLocationService _storageLocationService;
        private readonly ITranslationService _translationService;

        public StorageLocationsController(
            IStorageLocationService storageLocationService,
            ITranslationService translationService)
        {
            _storageLocationService = storageLocationService;
            _translationService = translationService;
        }

        // =========================
        // GET: api/storage-locations?lang=vi
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string lang = "vi")
        {
            var result = await _storageLocationService.GetAllAsync(lang);
            return Ok(result);
        }

        // =========================
        // GET: api/storage-locations/{id}?lang=en
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            string id,
            [FromQuery] string lang = "vi")
        {
            var result = await _storageLocationService
                .GetByIdAsync(id, lang);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // =========================
        // GET: api/storage-locations/statuses?lang=vi
        // Dropdown trạng thái
        // =========================
        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses(
            [FromQuery] string lang = "vi")
        {
            var statuses = await _translationService
                .GetByLanguageAsync(lang);

            var result = statuses
                .Where(x => x.Key.StartsWith("STORAGE_LOCATION_STATUS_"))
                .Select(x => new
                {
                    Status = int.Parse(
                        x.Key.Replace("STORAGE_LOCATION_STATUS_", "")
                    ),
                    Text = x.Value
                })
                .OrderBy(x => x.Status)
                .ToList();

            return Ok(result);
        }

        // =========================
        // PUT: api/storage-locations/{id}/status
        // =========================
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            string id,
            [FromBody] UpdateStorageLocationStatusRequest request)
        {
            await _storageLocationService
                .UpdateStatusAsync(id, request.Status);

            return NoContent();
        }
        [HttpGet("search")]
        public async Task<PagedResult<StorageLocationListDto>> Search(
    [FromQuery] SearchStorageLocationDto dto)
        {
            return await _storageLocationService.SearchAsync(dto);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateStorageLocationDto dto)
        {
            await _storageLocationService.CreateAsync(dto);
            return Ok();
        }

        [HttpPut("{storageLocationNo}")]
        public async Task<IActionResult> Update(
            string storageLocationNo,
            UpdateStorageLocationDto dto)
        {
            await _storageLocationService.UpdateAsync(storageLocationNo, dto);
            return Ok();
        }
    }

    // =========================
    // REQUEST DTO
    // =========================
    public class UpdateStorageLocationStatusRequest
    {
        public int Status { get; set; }
    }
}

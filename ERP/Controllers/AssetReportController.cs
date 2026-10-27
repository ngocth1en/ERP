using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Asset.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/asset-reports")]
    [EnableRateLimiting("ApiLimiter")]

    public class AssetReportController : ControllerBase
    {
        private readonly IAssetReportService _service;

        public AssetReportController(IAssetReportService service)
        {
            _service = service;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] AssetReportSearchDto dto)
        {
            var result = await _service.GetReportAsync(dto);
            return Ok(result);
        }

        [HttpPost("export")]
        public async Task<IActionResult> Export([FromBody] AssetReportSearchDto dto)
        {
            var bytes = await _service.ExportExcelAsync(dto);
            var fileName = $"BaoCaoTaiSan_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
    }
}
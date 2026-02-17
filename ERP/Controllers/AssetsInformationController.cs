using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Asset.Model.Interfaces;
using ERP.Domain.Entities;
using ERP.Infrastructure.ERP.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/assets-information")]
    public class AssetsInformationController : ControllerBase
    {
        private readonly IAssetInformationService _service;

        public AssetsInformationController(IAssetInformationService service)
        {
            _service = service;
        }

        // ================== STATISTIC ==================
        [HttpPost("statistics/search")]
        public async Task<IActionResult> SearchStatistic(
            [FromBody] AssetStatisticSearchDto dto)
        {
            var result = await _service.SearchStatisticAsync(dto);
            return Ok(result);
        }

        // ================== ASSETS BY MODEL ==================
        [HttpPost("by-model/search")]
        public async Task<IActionResult> SearchAssetsByModel(
            [FromBody] AssetByModelSearchDto dto)
        {
            var result = await _service.SearchAssetsByModelAsync(dto);
            return Ok(result);
        }

        // ================== CREATE ==================
        [HttpPost]
        public async Task<IActionResult> Create(
    [FromBody] CreateAssetDto dto,
    [FromQuery] string username
)
        {
            await _service.CreateAsync(dto, username);

            return Ok(new
            {
                message = "Tạo tài sản thành công"
            });
        }


        // ================== UPDATE ==================
        [HttpPut("{assetNo}")]
        public async Task<IActionResult> Update(
    string assetNo,
    [FromBody] UpdateAssetDto dto,
    [FromQuery] string username
)
        {
            await _service.UpdateAsync(assetNo, dto, username);

            return Ok(new
            {
                message = "Cập nhật tài sản thành công"
            });
        }

    }
}

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
        // ================== INVENTORY ORDERS ==================
        [HttpPost("inventory/orders")]
        public async Task<IActionResult> CreateInventoryOrder(
            [FromBody] CreateInventoryOrderDto dto,
            [FromQuery] string? username)
        {
            var result = await _service.CreateInventoryOrderAsync(dto, username);
            return Ok(result);
        }

        [HttpPut("inventory/orders/{id}")]
        public async Task<IActionResult> UpdateInventoryOrder(
            string id,
            [FromBody] UpdateInventoryOrderDto dto,
            [FromQuery] string? username)
        {
            var result = await _service.UpdateInventoryOrderAsync(id, dto, username);
            return Ok(result);
        }

        [HttpGet("inventory/orders")]
        public async Task<IActionResult> SearchInventoryOrders(
            [FromQuery] int? modelId,
            [FromQuery] string? departmentId,
            [FromQuery] int? pageIndex,
            [FromQuery] int? pageSize)
        {
            var result = await _service.SearchInventoryOrdersAsync(modelId, departmentId, pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet("inventory/orders/{id}")]
        public async Task<IActionResult> GetInventoryOrder(string id)
        {
            var result = await _service.GetInventoryOrderAsync(id);
            return Ok(result);
        }
        // ================== INVENTORY CHECK ==================
        [HttpPost("inventory/check")]
        public async Task<IActionResult> InventoryCheck(
            [FromBody] InventoryCheckRequestDto dto,
            [FromQuery] string username)
        {
            var result = await _service.InventoryCheckAsync(dto, username);
            return Ok(result);
        }
        [HttpGet("inventory/summary")]
        public async Task<IActionResult> GetInventorySummary(
    [FromQuery] int modelId,
    [FromQuery] string? departmentId,
    [FromQuery] int months = 6)
        {
            var username = HttpContext.User?.Identity?.Name ?? ""; // hoặc lấy từ query nếu cần
            var result = await _service.GetMonthlyInventorySummaryAsync(modelId, departmentId, months);
            return Ok(result);
        }
    }
}

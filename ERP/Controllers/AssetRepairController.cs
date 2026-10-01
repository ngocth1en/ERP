using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Asset.Model.Interfaces;
using ERP.Infrastructure.ERP.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/asset-repairs")]
    [Authorize]
    public class AssetRepairController : ControllerBase
    {
        private readonly IAssetRepairService _service;

        public AssetRepairController(IAssetRepairService service)
        {
            _service = service;
        }

        private string CurrentUser =>
            User.FindFirstValue("username") ?? throw new UnauthorizedAccessException();

        /// <summary>Tạo đơn sửa chữa cho 1 tài sản đã báo phế</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRepairDto dto)
            => Ok(await _service.CreateAsync(dto, CurrentUser));

        /// <summary>Tìm kiếm danh sách đơn</summary>
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] SearchRepairOrdersDto dto)
            => Ok(await _service.SearchAsync(dto));

        /// <summary>Lấy chi tiết 1 đơn</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
            => Ok(await _service.GetAsync(id));

        /// <summary>Sửa đơn (chỉ Draft / Pending)</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateRepairDto dto)
            => Ok(await _service.UpdateAsync(id, dto, CurrentUser));

        /// <summary>Bắt đầu sửa chữa</summary>
        [HttpPost("{id}/start")]
        public async Task<IActionResult> Start(string id)
            => Ok(await _service.StartAsync(id, CurrentUser));

        /// <summary>Hoàn thành sửa chữa → phục hồi tài sản</summary>
        [HttpPost("{id}/complete")]
        public async Task<IActionResult> Complete(string id)
            => Ok(await _service.CompleteAsync(id, CurrentUser));

        /// <summary>Hủy đơn sửa chữa</summary>
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(string id)
            => Ok(await _service.CancelAsync(id, CurrentUser));

        /// <summary>Tìm tài sản đã báo phế (Confirmed) để tạo đơn sửa chữa</summary>
        [HttpGet("scrapped-assets")]
        public async Task<IActionResult> SearchScrappedAssets([FromQuery] SearchScrappedAssetsDto dto)
            => Ok(await _service.SearchScrappedAssetsAsync(dto));
        /// <summary>Lịch sử sửa chữa (tất cả đơn, lọc theo tài sản/đơn vị/khoảng thời gian/trạng thái)</summary>
        [HttpGet("maintenance/history")]
        public async Task<IActionResult> GetMaintenanceHistory([FromQuery] MaintenanceHistoryQueryDto dto)
            => Ok(await _service.GetMaintenanceHistoryAsync(dto));

        /// <summary>Thống kê số lần sửa chữa và tổng chi phí theo từng tài sản</summary>
        [HttpGet("maintenance/summary-by-asset")]
        public async Task<IActionResult> GetMaintenanceSummaryByAsset([FromQuery] MaintenanceHistoryQueryDto dto)
            => Ok(await _service.GetMaintenanceSummaryByAssetAsync(dto));

        /// <summary>Thống kê tổng hợp (dashboard bảo trì máy)</summary>
        [HttpGet("maintenance/overall-statistic")]
        public async Task<IActionResult> GetOverallStatistic([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] int topCount = 10)
            => Ok(await _service.GetOverallStatisticAsync(fromDate, toDate, topCount));
    }
}

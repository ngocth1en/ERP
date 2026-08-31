using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Asset.Model.Interfaces;
using ERP.Infrastructure.ERP.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/asset-handovers")]
    [Authorize]
    public class AssetHandoverController : ControllerBase
    {
        private readonly IAssetHandoverService _service;

        public AssetHandoverController(IAssetHandoverService service)
        {
            _service = service;
        }

        private string CurrentUser =>
            User.FindFirstValue("username") ?? throw new UnauthorizedAccessException();

        // ─── PHIẾU BÀN GIAO ───────────────────────────────────────

        /// <summary>Tạo phiếu bàn giao kèm danh sách tài sản</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateHandoverDto dto)
            => Ok(await _service.CreateAsync(dto, CurrentUser));

        /// <summary>Tìm kiếm danh sách phiếu</summary>
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] SearchHandoverOrdersDto dto)
            => Ok(await _service.SearchAsync(dto));

        /// <summary>Lấy chi tiết 1 phiếu</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
            => Ok(await _service.GetAsync(id));

        /// <summary>Sửa phiếu (chỉ Draft / Pending)</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateHandoverDto dto)
            => Ok(await _service.UpdateAsync(id, dto, CurrentUser));

        // ─── HÀNH ĐỘNG ────────────────────────────────────────────

        /// <summary>Xác nhận phiếu → cập nhật đơn vị sử dụng tài sản</summary>
        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> Confirm(string id)
            => Ok(await _service.ConfirmAsync(id, CurrentUser));

        /// <summary>Hủy phiếu (gộp Reject + Cancel)</summary>
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(string id)
            => Ok(await _service.CancelAsync(id, CurrentUser));

        /// <summary>Hoàn trả tài sản (Unlock) → chuyển về đơn vị gốc</summary>
        [HttpPost("{id}/unlock")]
        public async Task<IActionResult> Unlock(string id)
            => Ok(await _service.UnlockAsync(id, CurrentUser));

        // ─── CHI TIẾT TÀI SẢN ─────────────────────────────────────

        /// <summary>Thêm 1 hoặc nhiều tài sản vào phiếu</summary>
        [HttpPost("{id}/details")]
        public async Task<IActionResult> AddAssets(string id, [FromBody] AddHandoverAssetsDto dto)
            => Ok(await _service.AddAssetsAsync(id, dto, CurrentUser));

        /// <summary>Xóa 1 tài sản khỏi phiếu</summary>
        [HttpDelete("{id}/details/{detailId:int}")]
        public async Task<IActionResult> RemoveAsset(string id, int detailId)
        {
            await _service.RemoveAssetAsync(id, detailId, CurrentUser);
            return NoContent();
        }

        // ─── TÌM KIẾM TÀI SẢN ────────────────────────────────────

        /// <summary>Tìm tài sản theo đơn vị và vị trí để thêm vào phiếu</summary>
        [HttpGet("candidate-assets")]
        public async Task<IActionResult> SearchCandidateAssets([FromQuery] SearchHandoverAssetsDto dto)
            => Ok(await _service.SearchCandidateAssetsAsync(dto));
    }
}

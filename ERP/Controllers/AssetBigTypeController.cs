using Azure.Core;
using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Asset.Model.Interfaces;
using ERP.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/asset-big-types")]
    public class AssetBigTypeController : ControllerBase
    {
        private readonly IAssetBigTypeService _service;

        public AssetBigTypeController(IAssetBigTypeService service)
        {
            _service = service;
        }

        /* ================= HELPER ================= */

        /// <summary>
        /// Lấy user hiện tại từ FE (không dùng Identity)
        /// </summary>
        private string CurrentUser =>
            Request.Headers["X-User"].FirstOrDefault() ?? "SYSTEM";

        /* ================= SEARCH ================= */

        /// <summary>
        /// Tìm kiếm loại thiết bị lớn
        /// - Tên EN
        /// - Tên VN
        /// - Phân trang
        /// </summary>
        [HttpGet("search")]
        public async Task<PagedResult<AssetBigTypeListDto>> Search(
            [FromQuery] SearchAssetBigTypeDto dto)
        {
            return await _service.SearchAsync(dto);
        }

        /* ================= CREATE ================= */

        /// <summary>
        /// Thêm mới loại thiết bị lớn
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateAssetBigTypeDto dto)
        {
            await _service.CreateAsync(dto, CurrentUser);
            return Ok();
        }

        /* ================= UPDATE ================= */

        /// <summary>
        /// Cập nhật loại thiết bị lớn
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateAssetBigTypeDto dto)
        {
            await _service.UpdateAsync(id, dto, CurrentUser);
            return Ok();
        }

        /* ================= DELETE (SOFT) ================= */

        /// <summary>
        /// Xóa mềm loại thiết bị lớn
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id, CurrentUser);
            return Ok();
        }
    }
}

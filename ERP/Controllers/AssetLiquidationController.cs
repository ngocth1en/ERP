using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Asset.Model.Interfaces;
using ERP.Infrastructure.ERP.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/asset-liquidations")]
    [Authorize]
    public class AssetLiquidationController : ControllerBase
    {
        private readonly IAssetLiquidationService _service;

        public AssetLiquidationController(IAssetLiquidationService service)
        {
            _service = service;
        }

        private string CurrentUser =>
            User.FindFirstValue("username") ?? throw new UnauthorizedAccessException();

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLiquidationDto dto)
            => Ok(await _service.CreateAsync(dto, CurrentUser));

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] SearchLiquidationOrdersDto dto)
            => Ok(await _service.SearchAsync(dto));

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
            => Ok(await _service.GetAsync(id));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateLiquidationDto dto)
            => Ok(await _service.UpdateAsync(id, dto, CurrentUser));

        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> Confirm(string id)
            => Ok(await _service.ConfirmAsync(id, CurrentUser));

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(string id)
            => Ok(await _service.CancelAsync(id, CurrentUser));

        [HttpPost("{id}/details")]
        public async Task<IActionResult> AddAssets(string id, [FromBody] AddLiquidationAssetsDto dto)
            => Ok(await _service.AddAssetsAsync(id, dto, CurrentUser));

        [HttpDelete("{id}/details/{detailId:int}")]
        public async Task<IActionResult> RemoveAsset(string id, int detailId)
        {
            await _service.RemoveAssetAsync(id, detailId, CurrentUser);
            return NoContent();
        }

        [HttpGet("scrapped-assets")]
        public async Task<IActionResult> SearchScrappedAssets([FromQuery] SearchScrappedAssetsDto dto)
            => Ok(await _service.SearchScrappedAssetsAsync(dto));
    }
}

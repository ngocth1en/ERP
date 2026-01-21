using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Asset.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers
{
    [ApiController]
    [Route("api/asset-models")]
    public class AssetModelController : ControllerBase
    {
        private readonly IAssetModelService _service;

        public AssetModelController(IAssetModelService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] AssetModelSearchDto dto)
        {
            var result = await _service.SearchAsync(dto);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateAssetModelDto dto,
            [FromQuery] string username)
        {
            var id = await _service.CreateAsync(dto, username);
            return Ok(id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateAssetModelDto dto,
            [FromQuery] string username)
        {
            await _service.UpdateAsync(id, dto, username);
            return NoContent();
        }
    }
}

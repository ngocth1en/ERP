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

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] AssetModelSearchDto dto)
        {
            var result = await _service.SearchAsync(dto);
            return Ok(result);
        }
        [HttpGet("options")]
        public async Task<IActionResult> GetOptions()
        {
            var result = await _service.GetOptionsAsync();
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

        /// <summary>
        /// Upload file model 3D (.glb/.gltf) cho CAD/3D Visualization.
        /// Trả về URL tương đối, dùng để lưu vào Model3DUrl khi Create/Update.
        /// </summary>
        [HttpPost("upload-3d")]
        public async Task<IActionResult> UploadModel3D([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ");

            await using var stream = file.OpenReadStream();
            var url = await _service.UploadModel3DAsync(file.FileName, stream);

            return Ok(new { url });
        }
    }
}
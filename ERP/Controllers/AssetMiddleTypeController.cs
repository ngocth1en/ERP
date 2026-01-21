using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Asset.Model.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/asset-middle-types")]
    //[Authorize]
    public class AssetMiddleTypeController : ControllerBase
    {
        private readonly IAssetMiddleTypeService _service;

        public AssetMiddleTypeController(IAssetMiddleTypeService service)
        {
            _service = service;
        }

        /* ================= SEARCH (TAB 2) ================= */
        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] AssetMiddleTypeSearchDto dto)
        {
            var result = await _service.SearchAsync(dto);
            return Ok(result);
        }

        /* ================= CREATE ================= */
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateAssetMiddleTypeDto dto, [FromQuery] string username)
        {

            var id = await _service.CreateAsync(dto, username);
            return Ok(id);
        }

        /* ================= UPDATE ================= */
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateAssetMiddleTypeDto dto, [FromQuery] string username)
        {

            await _service.UpdateAsync(id, dto, username);
            return NoContent();
        }

        /* ================= HIDE / SHOW ================= */
        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActive(int id, [FromQuery] string username)
        {
            await _service.ToggleActiveAsync(id, username);
            return NoContent();
        }
    }
}

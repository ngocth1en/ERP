using ERP.Application.Asset.Model.Dtos;
using ERP.Application.Asset.Model.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/asset-small-types")]
//[Authorize]
public class AssetSmallTypesController : ControllerBase
{
    private readonly IAssetSmallTypeService _service;

    public AssetSmallTypesController(IAssetSmallTypeService service)
    {
        _service = service;
    }

    /* ============================
       SEARCH
    ============================ */
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] AssetSmallTypeSearchDto dto)
    {
        var result = await _service.SearchAsync(dto);
        return Ok(result);
    }

    /* ============================
       CREATE
    ============================ */
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateAssetSmallTypeDto dto, [FromQuery] string username)
    {
        var id = await _service.CreateAsync(dto, username);
        return Ok(id);
    }

    /* ============================
       UPDATE
    ============================ */
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateAssetSmallTypeDto dto, [FromQuery] string username)
    {
        await _service.UpdateAsync(id, dto, username);
        return Ok();
    }

    /* ============================
       TOGGLE ACTIVE
    ============================ */
    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> ToggleActive(int id, [FromQuery] string username)
    {
        await _service.ToggleActiveAsync(id, username);
        return Ok();
    }
}

using EnergyBoard.Application.DTOs.request.columns;
using EnergyBoard.Application.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EnergyBoard.API.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId:int}/columns")]
public class ColumnController(IColumnService columnService) : ControllerBase
{
    private readonly IColumnService _columnService = columnService;

    private Guid GetUserId()
        => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll(int projectId)
    {
        return Ok(await _columnService.GetAllAsync(projectId, GetUserId()));
    }

    [HttpGet("{columnId:int}")]
    public async Task<IActionResult> GetById(int projectId, int columnId)
    {
        return Ok(await _columnService.GetByIdAsync(projectId, columnId, GetUserId()));
    }

    [HttpPost]
    public async Task<IActionResult> Create(int projectId, [FromBody] CreateColumnRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdId = await _columnService.AddAsync(projectId, request, GetUserId());

        return CreatedAtAction(nameof(GetById), 
            new { projectId, columnId = createdId },
            null);
    }

    [HttpPut("{columnId:int}")]
    public async Task<IActionResult> Update(
        int projectId, 
        int columnId, 
        [FromBody] UpdateColumnRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _columnService.UpdateAsync(projectId, columnId, request, GetUserId());

        return NoContent();
    }

    [HttpDelete("{columnId:int}")]
    public async Task<IActionResult> Delete(int projectId, int columnId)
    {
        await _columnService.DeleteAsync(projectId, columnId, GetUserId());

        return NoContent();
    }

    [HttpPatch("{columnId:int}/position")]
    public async Task<IActionResult> UpdatePosition(int projectId, int columnId,[FromBody] int newPosition)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _columnService.UpdatePositionAsync(projectId, columnId, newPosition, GetUserId());

        return NoContent();
    }
}
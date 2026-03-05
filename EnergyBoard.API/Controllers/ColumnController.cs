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
        var userId = GetUserId();
        var columns = await _columnService.GetAllAsync(projectId, userId);
        return Ok(columns);
    }

    [HttpGet("{columnId:int}")]
    public async Task<IActionResult> GetById(int projectId, int columnId)
    {
        var userId = GetUserId();
        var column = await _columnService.GetByIdAsync(projectId, columnId, userId);
        return Ok(column);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        int projectId, 
        [FromBody] CreateColumnRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();

        var createdId = await _columnService.AddAsync(projectId, request, userId);

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

        var userId = GetUserId();

        await _columnService.UpdateAsync(projectId, columnId, request, userId);

        return NoContent();
    }

    [HttpDelete("{columnId:int}")]
    public async Task<IActionResult> Delete(int projectId, int columnId)
    {
        var userId = GetUserId();

        await _columnService.DeleteAsync(projectId, columnId, userId);

        return NoContent();
    }

    [HttpPatch("{columnId:int}/position")]
    public async Task<IActionResult> UpdatePosition(
        int projectId,
        int columnId,
        [FromBody] MoveColumnRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();

        await _columnService.UpdatePositionAsync(projectId, columnId, request, userId);

        return NoContent();
    }
}
using EnergyBoard.Application.DTOs.request.projects;
using EnergyBoard.Application.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EnergyBoard.API.Controllers;

[Authorize]
[ApiController]
[Route("api/projects")]
public class ProjectController(IProjectService projectService) : ControllerBase
{
    private readonly IProjectService _projectService = projectService;

    private Guid GetUserId()
        => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _projectService.GetAllAsync(GetUserId()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, [FromQuery] bool includeDetails = false)
    {
        if (includeDetails)
            return Ok(await _projectService.GetCompleteAsync(id, GetUserId()));

        return Ok(await _projectService.GetByIdAsync(id, GetUserId()));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdId = await _projectService.AddAsync(request, GetUserId());

        return CreatedAtAction(nameof(GetById), 
            new { id = createdId }, null);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _projectService.UpdateAsync(id, request, GetUserId());

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _projectService.DeleteAsync(id, GetUserId());

        return NoContent();
    }

    [HttpPatch("{id}/position")]
    public async Task<IActionResult> UpdatePosition(int id, [FromBody] int newPosition)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _projectService.UpdatePositionAsync(id, newPosition, GetUserId());

        return NoContent();
    }
}
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
    public async Task<IActionResult> GetAllProjects()
    {
        return Ok(await _projectService.GetAllProjectsAsync(GetUserId()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProjectById(int id, [FromQuery] bool includeDetails = false)
    {
        if (includeDetails)
            return Ok(await _projectService.GetProjectCompleteAsync(id, GetUserId()));

        return Ok(await _projectService.GetProjectByIdAsync(id, GetUserId()));
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdId = await _projectService.AddProjectAsync(request, GetUserId());

        return CreatedAtAction(nameof(GetProjectById), new { id = createdId }, null);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _projectService.UpdateProjectAsync(id, request, GetUserId());

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        await _projectService.DeleteProjectAsync(id, GetUserId());
        return NoContent();
    }

    [HttpPatch("{id}/position")]
    public async Task<IActionResult> UpdateProjectPosition(int id, [FromBody] MoveProjectRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _projectService.UpdateProjectPositionAsync(id, request, GetUserId());

        return NoContent();
    }
}
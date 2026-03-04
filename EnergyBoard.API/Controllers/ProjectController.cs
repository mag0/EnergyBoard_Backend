using Microsoft.AspNetCore.Mvc;
using EnergyBoard.Application.services;

namespace EnergyBoard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController (ProjectService projectService) : ControllerBase
{
    private readonly ProjectService _projectServiec;

    [HttpGet]
    public async Task<IActionResult> GetAllProjects()
    {
        var projects = await _projectServiec.GetAllProjectsAsync();
        return Ok(projects);
    }

}

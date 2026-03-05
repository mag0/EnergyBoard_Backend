
using EnergyBoard.Application.DTOs.request.projects;
using EnergyBoard.Application.DTOs.response.projects;

namespace EnergyBoard.Application.interfaces;

public interface IProjectService
{
    Task<int> AddProjectAsync(CreateProjectRequest request, Guid userId);
    Task<IEnumerable<ProjectResponse>> GetAllProjectsAsync(Guid userId);
    Task<ProjectResponse?> GetProjectByIdAsync(int id, Guid userId);
    Task<CompleteProjectResponse?> GetProjectCompleteAsync(int id, Guid userId);
    Task UpdateProjectAsync(int id, UpdateProjectRequest request, Guid userId);
    Task DeleteProjectAsync(int id, Guid userId);
    Task UpdateProjectPositionAsync(int id, MoveProjectRequest request, Guid userId);
}

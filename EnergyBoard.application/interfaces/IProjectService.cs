
using EnergyBoard.Application.DTOs.request.projects;
using EnergyBoard.Application.DTOs.response.projects;

namespace EnergyBoard.Application.interfaces;

public interface IProjectService
{
    Task<int> AddAsync(CreateProjectRequest request, Guid userId);
    Task<IEnumerable<ProjectResponse>> GetAllAsync(Guid userId);
    Task<ProjectResponse> GetByIdAsync(int projectId, Guid userId);
    Task<CompleteProjectResponse> GetCompleteAsync(int projectId, Guid userId);
    Task UpdateAsync(int projectId, UpdateProjectRequest request, Guid userId);
    Task DeleteAsync(int projectId, Guid userId);
    Task UpdatePositionAsync(int projectId, int newPosition, Guid userId);
}

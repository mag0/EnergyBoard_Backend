
using EnergyBoard.Domain.entities;

namespace EnergyBoard.Domain.interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(int projectId, Guid userId);
    Task<List<Project>> GetAllAsync(Guid userId);
    Task AddAsync(Project project);
    Task UpdateAsync(Project project);
    Task UpdateRangeAsync(List<Project> projects);
    Task<Project?> GetCompleteProjectAsync(int projectId, Guid userId);
    Task<bool> ExistsAsync( int projectId, Guid userId);
    Task<int> GetNextPositionAsync(Guid userId);
}

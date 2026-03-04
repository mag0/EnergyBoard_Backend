
using EnergyBoard.Domain.entities;

namespace EnergyBoard.Domain.interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(int id);
    Task<IEnumerable<Project>> GetAllAsync();
    Task AddAsync(Project project);
    Task UpdateAsync(Project project);
    Task DeleteAsync(Project project);
    Task<Project?> GetProjectCompleteAsync(int id);
    Task UpdateRangeAsync(IEnumerable<Project> projects);
    Task<int> GetNextPositionAsync();
}

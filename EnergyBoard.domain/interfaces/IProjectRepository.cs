
using EnergyBoard.Domain.entities;

namespace EnergyBoard.Domain.interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(int id, Guid userId);
    Task<IEnumerable<Project>> GetAllAsync(Guid userId);
    Task AddAsync(Project project);
    Task UpdateAsync(Project project);
    Task<Project?> GetCompleteProjectAsync(int id, Guid userId);
    Task UpdateRangeAsync(IEnumerable<Project> projects);
}

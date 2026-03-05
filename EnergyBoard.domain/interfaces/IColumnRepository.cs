
using EnergyBoard.Domain.entities;

namespace EnergyBoard.Domain.interfaces;

public interface IColumnRepository
{
    Task<Column?> GetByIdAsync(int projectId, int columnId, Guid userId);
    Task<List<Column>> GetAllAsync(int projectId, Guid userId);
    Task AddAsync(Column column);
    Task UpdateAsync(Column column);
    Task UpdateRangeAsync(IEnumerable<Column> columns);
    Task<int> GetNextPositionAsync(int projectId, Guid userId);
    Task<bool> ExistsAsync(int columnId, int projectId, Guid userId);
}


using EnergyBoard.Domain.entities;

namespace EnergyBoard.Domain.interfaces;

public interface IColumnRepository
{
    Task<Column?> GetByIdAsync(int id);
    Task<IEnumerable<Column>> GetAllAsync();
    Task AddAsync(Column column);
    Task UpdateAsync(Column column);
    Task DeleteAsync(Column column);
    Task UpdateRangeAsync(IEnumerable<Column> columns);
    Task<int> GetNextPositionAsync();
}

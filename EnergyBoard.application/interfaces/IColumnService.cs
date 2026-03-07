
using EnergyBoard.Application.DTOs.request.columns;
using EnergyBoard.Application.DTOs.response.columns;

namespace EnergyBoard.Application.interfaces;

public interface IColumnService
{
    Task<int> AddAsync(int projectId, CreateColumnRequest request, Guid userId);
    Task<IEnumerable<ColumnResponse>> GetAllAsync(int projectId, Guid userId);
    Task<ColumnResponse> GetByIdAsync(int projectId, int columnId, Guid userId);
    Task UpdateAsync(int projectId, int columnId, UpdateColumnRequest request, Guid userId);
    Task DeleteAsync(int projectId, int columnId, Guid userId);
    Task UpdatePositionAsync(int projectId, int columnId, int newPosition, Guid userId);
}

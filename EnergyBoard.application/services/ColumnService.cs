
using AutoMapper;
using EnergyBoard.Application.DTOs.request.columns;
using EnergyBoard.Application.DTOs.response.columns;
using EnergyBoard.Application.interfaces;
using EnergyBoard.Domain.entities;
using EnergyBoard.Domain.interfaces;

namespace EnergyBoard.Application.services;

public class ColumnService (IColumnRepository columnRepository, IProjectRepository projectRepository, IMapper mapper) : IColumnService
{
    private readonly IColumnRepository _columnRepository = columnRepository;
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<int> AddAsync(int projectId, CreateColumnRequest request, Guid userId)
    {
        await ValidateProjectAsync(projectId, userId);

        var nextPosition = await _columnRepository.GetNextPositionAsync(projectId, userId);

        var column = _mapper.Map<Column>(request);

        column.ProjectId = projectId;
        column.Position = nextPosition;
        column.CreatedAt = DateTime.UtcNow;

        await _columnRepository.AddAsync(column);

        return column.Id;
    }

    public async Task<IEnumerable<ColumnResponse>> GetAllAsync(int projectId, Guid userId)
    {
        await ValidateProjectAsync(projectId, userId);

        var columns = await _columnRepository.GetAllAsync(projectId, userId);
        return _mapper.Map<IEnumerable<ColumnResponse>>(columns);
    }

    public async Task<ColumnResponse> GetByIdAsync(int projectId, int columnId, Guid userId)
    {
        await ValidateProjectAsync(projectId, userId);

        var column = await _columnRepository.GetByIdAsync(projectId, columnId,  userId)
            ?? throw new KeyNotFoundException("Column not found");
        return _mapper.Map<ColumnResponse>(column);
    }

    public async Task UpdateAsync(int projectId, int columnId, UpdateColumnRequest request, Guid userId)
    {
        await ValidateProjectAsync(projectId, userId);

        var column = await GetEntityAsync(projectId, columnId, userId);
        
        column.Title = request.Title ?? column.Title;
        column.Description = request.Description ?? column.Description;
        column.UpdatedAt = DateTime.UtcNow;

        await _columnRepository.UpdateAsync(column);
    }

    public async Task DeleteAsync(int projectId, int columnId, Guid userId)
    {
        await ValidateProjectAsync(projectId, userId);

        var column = await GetEntityAsync(projectId, columnId, userId);

        column.IsDeleted = true;
        column.UpdatedAt = DateTime.UtcNow;

        await _columnRepository.UpdateAsync(column);
    }

    public async Task UpdatePositionAsync(int projectId, int columnId, int newPosition, Guid userId)
    {
        await ValidateProjectAsync(projectId, userId);

        var columns = await _columnRepository.GetAllAsync(projectId, userId);

        var columnToMove = columns.FirstOrDefault(c => c.Id == columnId)
            ?? throw new KeyNotFoundException("Column not found");

        columns.Remove(columnToMove);

        var newIndex = Math.Clamp(newPosition - 1, 0, columns.Count);
        columns.Insert(newIndex, columnToMove);

        for (int i = 0; i < columns.Count; i++)
        {
            columns[i].Position = i + 1;
        }

        await _columnRepository.UpdateRangeAsync(columns);
    }

    private async Task<Column> GetEntityAsync(int projectId, int columnId, Guid userId)
    {
        return await _columnRepository.GetByIdAsync(projectId, columnId, userId)
            ?? throw new KeyNotFoundException("Column not found");
    }

    private async Task ValidateProjectAsync(int projectId, Guid userId)
    {
        if (!await _projectRepository.ExistsAsync(projectId, userId))
            throw new KeyNotFoundException("Project not found");
    }
}

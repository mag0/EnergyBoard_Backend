
using AutoMapper;
using EnergyBoard.Application.DTOs.request;
using EnergyBoard.Domain.entities;
using EnergyBoard.Domain.interfaces;

namespace EnergyBoard.Application.services;

public class ColumnService (IColumnRepository columnRepository, IMapper mapper)
{
    private readonly IColumnRepository _columnRepository = columnRepository;
    private readonly IMapper _mapper = mapper;

    public async Task AddColumnAsync(CreateColumnRequest createColumnRequest)
    {
        var nextPosition = await _columnRepository.GetNextPositionAsync();

        var column = _mapper.Map<Column>(createColumnRequest);

        column.Position = nextPosition;
        column.CreatedAt = DateTime.UtcNow;

        await _columnRepository.AddAsync(column);
    }

    public async Task UpdateColumnAsync(int id, UpdateColumnRequest updateColumn)
    {
        var column = await GetExistingColumn(id);
        
        if(updateColumn.Title != null)
        {
            column.Title = updateColumn.Title;
        }
        if(updateColumn.Description != null)
        {
            column.Description = updateColumn.Description;
        }
        await _columnRepository.UpdateAsync(column);
    }

    public async Task DeleteColumnAsync(int id)
    {
        var column = await GetExistingColumn(id);
        await _columnRepository.DeleteAsync(column);
    }

    public async Task UpdateProjectPositionAsync(int id, MoveColumnRequest request)
    {
        var columns = (await _columnRepository.GetAllAsync()).ToList();

        var columnToMove = columns.FirstOrDefault(p => p.Id == id)
            ?? throw new KeyNotFoundException("Column not found");

        columns.Remove(columnToMove);

        var newIndex = request.NewPosition;

        if (newIndex < 0)
            newIndex = 0;

        if (newIndex > columns.Count)
            newIndex = columns.Count;

        columns.Insert(newIndex, columnToMove);

        for (int i = 0; i < columns.Count; i++)
        {
            columns[i].Position = i + 1;
        }

        await _columnRepository.UpdateRangeAsync(columns);
    }

    private async Task<Column> GetExistingColumn(int id)
    {
        var column = await _columnRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Column not found");
        return column;
    }
}

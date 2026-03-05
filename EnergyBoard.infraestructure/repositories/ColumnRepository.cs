
using EnergyBoard.Domain.entities;
using EnergyBoard.Domain.interfaces;
using EnergyBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnergyBoard.Infrastructure.repositories;

public class ColumnRepository (AppDbContext context) : IColumnRepository
{
    private readonly AppDbContext _context = context;
    public async Task AddAsync(Column column)
    {
        _context.Columns.Add(column);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int columnId, int projectId, Guid userId)
    {
        return await _context.Columns
            .AnyAsync(c =>
                c.Id == columnId &&
                c.ProjectId == projectId &&
                c.Project.UserId == userId
            );
    }

    public async Task<List<Column>> GetAllAsync(int projectId, Guid userId)
    {
        return await _context.Columns
            .AsNoTracking()
            .Where(c =>
                c.ProjectId == projectId &&
                c.Project.UserId == userId)
            .OrderBy(c => c.Position)
            .ToListAsync();
    }

    public async Task<Column?> GetByIdAsync(int projectId, int columnId, Guid userId)
    {
        return await _context.Columns
            .AsNoTracking()
            .Where(c =>
                c.Id == columnId &&
                c.ProjectId == projectId &&
                c.Project.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetNextPositionAsync(int projectId, Guid userId)
    {
        var columns = _context.Columns
            .Where(c => 
                c.ProjectId == projectId &&
                c.Project.UserId == userId);

        var max = await columns.MaxAsync(c => (int?)c.Position);

        return (max ?? 0) + 1;
    }

    public async Task UpdateAsync(Column column)
    {
        _context.Columns.Update(column);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(IEnumerable<Column> columns)
    {
        _context.Columns.UpdateRange(columns);
        await _context.SaveChangesAsync();
    }
}

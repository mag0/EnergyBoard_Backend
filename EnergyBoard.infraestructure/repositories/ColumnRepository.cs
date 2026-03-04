
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

    public async Task DeleteAsync(Column column)
    {
        await UpdateAsync(column);
    }

    public async Task<Column?> GetByIdAsync(int id)
    {
        return await _context.Columns.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Column>> GetAllAsync()
    {
        return await _context.Columns.OrderBy(c => c.Position).ToListAsync();
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

    public async Task<int> GetNextPositionAsync()
    {
        if (!await _context.Columns.AnyAsync())
            return 1;

        return await _context.Columns.MaxAsync(p => p.Position) + 1;
    }
}

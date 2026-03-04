
using EnergyBoard.Domain.entities;
using EnergyBoard.Domain.interfaces;
using EnergyBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnergyBoard.Infrastructure.repositories;

public class ProjectRepository(AppDbContext context) : IProjectRepository
{
    private readonly AppDbContext _context = context;

    public async Task AddAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Project project)
    {
        await UpdateAsync(project);
    }

    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        return await _context.Projects.OrderBy(p => p.Position).ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(int id)
    {
        return await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Project?> GetProjectCompleteAsync(int id)
    {
        return await _context.Projects
            .Include(p => p.Columns)
            .ThenInclude(c => c.Cards)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task UpdateAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(IEnumerable<Project> projects)
    {
        _context.Projects.UpdateRange(projects);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetNextPositionAsync()
    {
        if (!await _context.Projects.AnyAsync())
            return 1;

        return await _context.Projects.MaxAsync(p => p.Position) + 1;
    }
}


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

    public async Task<bool> ExistsAsync(int projectId, Guid userId)
    {
        return await _context.Projects
            .AnyAsync(c =>
                c.Id == projectId &&
                c.UserId == userId
            );
    }

    public async Task<List<Project>> GetAllAsync(Guid userId)
    {
        return await _context.Projects
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .OrderBy(p => p.Position)
            .ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(int projectId, Guid userId)
    {
        return await _context.Projects
            .AsNoTracking()
            .Where(p => 
                p.UserId == userId &&
                p.Id == projectId)
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetNextPositionAsync(Guid userId)
    {
        var projects = _context.Projects
            .Where(c =>c.UserId == userId);

        var max = await projects.MaxAsync(c => (int?)c.Position);

        return (max ?? 0) + 1;
    }

    public async Task<Project?> GetCompleteProjectAsync(int projectId, Guid userId)
    {
        return await _context.Projects
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .Include(p => p.Columns)
            .ThenInclude(c => c.Cards)
            .FirstOrDefaultAsync(p => p.Id == projectId);
    }

    public async Task UpdateAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<Project> projects)
    {
        _context.Projects.UpdateRange(projects);
        await _context.SaveChangesAsync();
    }

}

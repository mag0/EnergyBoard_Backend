
using EnergyBoard.Domain.entities;
using EnergyBoard.Domain.interfaces;
using EnergyBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnergyBoard.Infrastructure.repositories;

public class CardRepository(AppDbContext context) : ICardRepository
{
    private readonly AppDbContext _context = context;

    public async Task AddAsync(Card card)
    {
        _context.Cards.Add(card);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Card>> GetAllAsync(int projectId, int columnId, Guid userId)
    {
        return await _context.Cards
            .AsNoTracking()
            .Where(c =>
                c.ColumnId == columnId &&
                c.Column.ProjectId == projectId && 
                c.Column.Project.UserId == userId)
            .OrderBy(c => c.Position)
            .ToListAsync();
    }

    public async Task<Card?> GetByIdAsync(int projectId, int columnId, int cardId, Guid userId)
    {
        return await _context.Cards
            .AsNoTracking()
            .Include(c => c.Column)
            .Where(c =>
                c.Id == cardId &&
                c.ColumnId == columnId &&
                c.Column.ProjectId == projectId && 
                c.Column.Project.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetNextPositionAsync(int projectId, int columnId, Guid userId)
    {
        var cards = _context.Cards
            .Where(c =>
                c.ColumnId == columnId &&
                c.Column.ProjectId == projectId &&
                c.Column.Project.UserId == userId);

        var max = await cards.MaxAsync(c => (int?)c.Position);

        return (max ?? 0) + 1;
    }

    public async Task UpdateAsync(Card card)
    {
        _context.Cards.Update(card);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<Card> cards)
    {
        _context.Cards.UpdateRange(cards);
        await _context.SaveChangesAsync();
    }
}

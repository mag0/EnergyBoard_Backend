
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

    public async Task DeleteAsync(Card card)
    {
        await UpdateAsync(card);
    }

    public async Task<IEnumerable<Card>> GetAllAsync()
    {
        return await _context.Cards.OrderBy(c => c.Position).ToListAsync();
    }

    public async Task<Card?> GetByIdAsync(int id)
    {
        return await _context.Cards.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task UpdateAsync(Card card)
    {
        _context.Cards.Update(card);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(IEnumerable<Card> cards)
    {
        _context.Cards.UpdateRange(cards);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetNextPositionAsync()
    {
        if (!await _context.Cards.AnyAsync())
            return 1;

        return await _context.Cards.MaxAsync(p => p.Position) + 1;
    }

    public async Task<IEnumerable<Card>> GetCardsByColumnIdAsync(int columnId)
    {
        return await _context.Cards.Where(c => c.ColumnId == c.ColumnId).OrderBy(c => c.Position).ToListAsync();
    }
}

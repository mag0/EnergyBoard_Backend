
using EnergyBoard.Domain.entities;

namespace EnergyBoard.Domain.interfaces;

public interface ICardRepository
{
    Task<IEnumerable<Card>> GetAllAsync();
    Task<Card?> GetByIdAsync(int id);
    Task AddAsync(Card card);
    Task UpdateAsync(Card card);
    Task DeleteAsync(Card card);
    Task UpdateRangeAsync(IEnumerable<Card> cards);
    Task<int> GetNextPositionAsync();
    Task<IEnumerable<Card>> GetCardsByColumnIdAsync(int columnId);
}

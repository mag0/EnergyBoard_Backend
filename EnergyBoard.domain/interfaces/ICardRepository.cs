
using EnergyBoard.Domain.entities;

namespace EnergyBoard.Domain.interfaces;

public interface ICardRepository
{
    Task<Card?> GetByIdAsync(int projectId, int columnId, int cardId, Guid userId);
    Task<List<Card>> GetAllAsync(int projectId, int columnId, Guid userId);
    Task AddAsync(Card card);
    Task UpdateAsync(Card card);
    Task UpdateRangeAsync(IEnumerable<Card> cards);
    Task<int> GetNextPositionAsync(int projectId, int columnId, Guid userId);
}

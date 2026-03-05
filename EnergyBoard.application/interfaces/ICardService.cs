
using EnergyBoard.Application.DTOs.request.cards;
using EnergyBoard.Application.DTOs.response.cards;

namespace EnergyBoard.Application.interfaces;

public interface ICardService
{
    Task<int> AddAsync(int projectId, int columnId, CreateCardRequest request, Guid userId);
    Task<IEnumerable<CardResponse>> GetAllAsync(int projectId, int columnId, Guid userId);
    Task<CardResponse> GetByIdAsync(int projectId, int columnId, int cardId, Guid userId);
    Task UpdateAsync(int projectId, int columnId, int cardId, UpdateCardRequest updateCard, Guid userId);
    Task DeleteAsync(int projectId, int columnId, int cardId, Guid userId);
    Task UpdatePositionAsync(int projectId, int columnId, int cardId, MoveCardRequest request, Guid userId);
}

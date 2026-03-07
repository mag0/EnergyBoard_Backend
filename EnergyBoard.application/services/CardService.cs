
using AutoMapper;
using EnergyBoard.Application.DTOs.request.cards;
using EnergyBoard.Application.DTOs.response.cards;
using EnergyBoard.Application.interfaces;
using EnergyBoard.Domain.entities;
using EnergyBoard.Domain.interfaces;

namespace EnergyBoard.Application.services;

public class CardService(ICardRepository cardRepository, IColumnRepository columnRepository, IMapper mapper) : ICardService
{
    private readonly ICardRepository _cardRepository = cardRepository;
    private readonly IColumnRepository _columnRepository = columnRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<int> AddAsync(int projectId, int columnId, CreateCardRequest request, Guid userId)
    {
        await ValidateColumnAsync(projectId, columnId, userId);

        var nextPosition = await _cardRepository.GetNextPositionAsync(projectId, columnId, userId);

        var card = _mapper.Map<Card>(request);
        card.ColumnId = columnId;
        card.Position = nextPosition;
        card.CreatedAt = DateTime.UtcNow;

        await _cardRepository.AddAsync(card);

        return card.Id;
    }

    public async Task<IEnumerable<CardResponse>> GetAllAsync(int projectId, int columnId, Guid userId)
    {
        await ValidateColumnAsync(projectId, columnId, userId);

        var cards = await _cardRepository.GetAllAsync(projectId, columnId, userId);
        return _mapper.Map<IEnumerable<CardResponse>>(cards);
    }

    public async Task<CardResponse> GetByIdAsync(int projectId, int columnId, int cardId, Guid userId)
    {
        await ValidateColumnAsync(projectId, columnId, userId);

        var card = await _cardRepository.GetByIdAsync(projectId, columnId, cardId, userId)
            ?? throw new KeyNotFoundException("Card not found");

        return _mapper.Map<CardResponse>(card);
    }

    public async Task UpdateAsync(int projectId, int columnId, int cardId, UpdateCardRequest updateCard, Guid userId)
    {
        await ValidateColumnAsync(projectId, columnId, userId);

        var card = await GetEntityAsync(projectId, columnId, cardId, userId);

        card.Title = updateCard.Title ?? card.Title;
        card.Description = updateCard.Description ?? card.Description;
        card.Deadline = updateCard.Deadline ?? card.Deadline;
        card.UpdatedAt = DateTime.UtcNow;

        await _cardRepository.UpdateAsync(card);
    }

    public async Task DeleteAsync(int projectId, int columnId, int cardId, Guid userId)
    {
        await ValidateColumnAsync(projectId, columnId, userId);

        var card = await GetEntityAsync(projectId, columnId, cardId, userId);
        card.IsDeleted = true;
        card.UpdatedAt = DateTime.UtcNow;
        await _cardRepository.UpdateAsync(card);
    }

    public async Task UpdatePositionAsync(int projectId, int columnId, int cardId, MoveCardRequest request, Guid userId)
    {
        await ValidateColumnAsync(projectId, columnId, userId);

        await (request.NewColumnId == null
            ? MoveWithinColumnAsync(projectId, columnId, cardId, request.NewPosition, userId)
            : MoveToAnotherColumnAsync(projectId, columnId, cardId, request.NewColumnId.Value, request.NewPosition, userId));
    }

    private async Task MoveWithinColumnAsync(int projectId, int columnId, int cardId, int newPosition, Guid userId)
    {
        var cards = await _cardRepository.GetAllAsync(projectId, columnId, userId);

        var cardToMove = cards.FirstOrDefault(c => c.Id == cardId)
            ?? throw new KeyNotFoundException("Card not found");

        cards.Remove(cardToMove);

        var newIndex = Math.Clamp(newPosition - 1, 0, cards.Count);
        cards.Insert(newIndex, cardToMove);

        Reassign(cards);

        await _cardRepository.UpdateRangeAsync(cards);
    }

    private async Task MoveToAnotherColumnAsync(int projectId, int sourceColumnId, int cardId, int targetColumnId, int newPosition, Guid userId)
    {
        var sourceCards = await _cardRepository.GetAllAsync(projectId, sourceColumnId, userId);

        var cardToMove = sourceCards.FirstOrDefault(c => c.Id == cardId)
            ?? throw new KeyNotFoundException("Card not found");

        sourceCards.Remove(cardToMove);

        var targetCards = await _cardRepository.GetAllAsync(projectId, targetColumnId, userId);

        var newIndex = Math.Clamp(newPosition - 1, 0, targetCards.Count);
        targetCards.Insert(newIndex, cardToMove);

        Reassign(sourceCards);
        Reassign(targetCards);

        cardToMove.ColumnId = targetColumnId;

        await _cardRepository.UpdateRangeAsync(sourceCards);
        await _cardRepository.UpdateRangeAsync(targetCards);
    }

    private static void Reassign(List<Card> cards)
    {
        for (int i = 0; i < cards.Count; i++)
            cards[i].Position = i + 1;
    }

    private async Task<Card> GetEntityAsync(int projectId, int columnId, int cardId, Guid userId)
    {
        return await _cardRepository.GetByIdAsync(projectId, columnId, cardId, userId)
            ?? throw new KeyNotFoundException("Card not found");
    }

    private async Task ValidateColumnAsync(int projectId, int columnId, Guid userId)
    {
        if (!await _columnRepository.ExistsAsync(projectId, columnId, userId))
            throw new KeyNotFoundException("Column not found");
    }
}

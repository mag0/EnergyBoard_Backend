
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
        var cards = await _cardRepository.GetAllAsync(projectId, columnId, userId);
        return _mapper.Map<IEnumerable<CardResponse>>(cards);
    }

    public async Task<CardResponse> GetByIdAsync(int projectId, int columnId, int cardId, Guid userId)
    {
        var card = await _cardRepository.GetByIdAsync(projectId, columnId, cardId, userId)
            ?? throw new KeyNotFoundException("Card not found");

        return _mapper.Map<CardResponse>(card);
    }

    public async Task UpdateAsync(int projectId, int columnId, int cardId, UpdateCardRequest updateCard, Guid userId)
    {
        var card = await GetEntityAsync(projectId, columnId, cardId, userId);

        card.Title = updateCard.Title ?? card.Title;
        card.Description = updateCard.Description ?? card.Description;
        card.Deadline = updateCard.Deadline ?? card.Deadline;
        card.UpdatedAt = DateTime.UtcNow;

        await _cardRepository.UpdateAsync(card);
    }

    public async Task DeleteAsync(int projectId, int columnId, int cardId, Guid userId)
    {
        var card = await GetEntityAsync(projectId, columnId, cardId, userId);
        card.IsDeleted = true;
        card.UpdatedAt = DateTime.UtcNow;
        await _cardRepository.UpdateAsync(card);
    }

    public async Task UpdatePositionAsync(int projectId, int columnId, int cardId, MoveCardRequest request, Guid userId)
    {
        var card = await GetEntityAsync(projectId, columnId, cardId, userId);

        var targetColumnId = request.NewColumnId ?? card.ColumnId;

        if (!await _columnRepository.ExistsAsync(targetColumnId, projectId, userId))
            throw new KeyNotFoundException("Target column not found");

        // mover dentro de la misma columna
        if (card.ColumnId == targetColumnId)
        {
            var cards = (await _cardRepository.GetAllAsync(projectId, card.ColumnId, userId)).ToList();
            ReorderList(cards, cardId, request.NewPosition);
            await _cardRepository.UpdateRangeAsync(cards);
            return;
        }

        // mover a otra columna
        var originCards = (await _cardRepository.GetAllAsync(projectId, card.ColumnId, userId)).ToList();
        originCards.RemoveAll(c => c.Id == cardId);
        RecalculatePositions(originCards);
        await _cardRepository.UpdateRangeAsync(originCards);

        var targetCards = (await _cardRepository.GetAllAsync(projectId, targetColumnId, userId)).ToList();
        card.ColumnId = targetColumnId;
        ReorderList(targetCards, card, request.NewPosition);
        await _cardRepository.UpdateRangeAsync(targetCards);
    }

    private void ReorderList(List<Card> cards, int cardId, int newPosition)
    {
        var card = cards.First(c => c.Id == cardId);
        ReorderList(cards, card, newPosition);
    }

    private void ReorderList(List<Card> cards, Card card, int newPosition)
    {
        cards.Remove(card);

        if (newPosition < 0) newPosition = 0;
        if (newPosition > cards.Count) newPosition = cards.Count;

        cards.Insert(newPosition, card);
        RecalculatePositions(cards);
    }

    private void RecalculatePositions(List<Card> cards)
    {
        for (int i = 0; i < cards.Count; i++)
            cards[i].Position = i + 1;
    }

    private async Task<Card> GetEntityAsync(int projectId, int columnId, int cardId, Guid userId)
    {
        return await _cardRepository.GetByIdAsync(projectId, columnId, cardId, userId)
            ?? throw new KeyNotFoundException("Card not found");
    }
}

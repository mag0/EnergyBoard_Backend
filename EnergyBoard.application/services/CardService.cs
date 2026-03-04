
using AutoMapper;
using EnergyBoard.Application.DTOs.request;
using EnergyBoard.Application.DTOs.response;
using EnergyBoard.Domain.entities;
using EnergyBoard.Domain.interfaces;

namespace EnergyBoard.Application.services;

public class CardService(ICardRepository cardRepository, IMapper mapper)
{
    private readonly ICardRepository _cardRepository = cardRepository;
    private readonly IMapper _mapper = mapper;

    public async Task AddCardAsync(CreateCardRequest createCardRequest)
    {
        var nextPosition = await _cardRepository.GetNextPositionAsync();

        var card = _mapper.Map<Card>(createCardRequest);

        card.Position = nextPosition;
        card.CreatedAt = DateTime.UtcNow;

        await _cardRepository.AddAsync(card);
    }

    public async Task<CardResponse?> GetCardByIdAsync(int id)
    {
        var card = await GetExistingCard(id);

        var cardResponse = _mapper.Map<CardResponse>(card);

        return cardResponse;
    }

    public async Task UpdateCardAsync(int id, UpdateCardRequest updateCard)
    {
        var card = await GetExistingCard(id);

        if (updateCard.Title != null)
        {
            card.Title = updateCard.Title;
        }
        if (updateCard.Description != null)
        {
            card.Description = updateCard.Description;
        }
        if(updateCard.Deadline != null)
        {
            card.Deadline = (DateTime)updateCard.Deadline;
        }
        card.UpdatedAt = DateTime.UtcNow;

        await _cardRepository.UpdateAsync(card);
    }

    public async Task<IEnumerable<CardResponse>> GetAllCardsAsync()
    {
        var cards = await _cardRepository.GetAllAsync();
        var cardResponses = cards.Select(card => _mapper.Map<CardResponse>(card));

        return cardResponses;
    }

    public async Task DeleteCardAsync(int id)
    {
        var card = await GetExistingCard(id);

        await _cardRepository.DeleteAsync(card);
    }

    public async Task UpdateCardPositionAsync(int id, MoveCardRequest request)
    {
        var cards = (await _cardRepository.GetCardsByColumnIdAsync(request.OriginalColumnId)).ToList();

        var cardToMove = cards.FirstOrDefault(c => c.Id == id)
            ?? throw new KeyNotFoundException("Card not found");

        cards.Remove(cardToMove);

        int newIndex = request.NewPosition;
        if (newIndex < 0) newIndex = 0;
        if (newIndex > cards.Count) newIndex = cards.Count;

        cards.Insert(newIndex, cardToMove);

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].Position = i + 1;
        }

        await _cardRepository.UpdateRangeAsync(cards);

        if (request.NewColumnId != null && request.NewColumnId != request.OriginalColumnId)
        {
            cardToMove.ColumnId = (int)request.NewColumnId;

            var newColumnCards = (await _cardRepository.GetCardsByColumnIdAsync((int)request.NewColumnId)).ToList();
            newColumnCards.Add(cardToMove);

            for (int i = 0; i < newColumnCards.Count; i++)
            {
                newColumnCards[i].Position = i + 1;
            }

            await _cardRepository.UpdateRangeAsync(newColumnCards);
        }
    }

    private async Task<Card> GetExistingCard(int id)
    {
        return await _cardRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Card not found");
    }
}

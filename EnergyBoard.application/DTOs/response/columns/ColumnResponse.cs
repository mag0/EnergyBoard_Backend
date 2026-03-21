using EnergyBoard.Application.DTOs.response.cards;

namespace EnergyBoard.Application.DTOs.response.columns;

public class ColumnResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Position { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ColumnWithCardsResponse : ColumnResponse
{
    public List<CardResponse> Cards { get; set; } = [];
}
namespace EnergyBoard.Application.DTOs.response;

public class CompleteProjectResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Position { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ColumnResponse> Columns { get; set; } = [];
}

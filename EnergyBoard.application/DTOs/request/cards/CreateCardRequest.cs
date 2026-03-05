
namespace EnergyBoard.Application.DTOs.request.cards;

public class CreateCardRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
}

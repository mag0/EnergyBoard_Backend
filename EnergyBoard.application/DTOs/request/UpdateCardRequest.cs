
namespace EnergyBoard.Application.DTOs.request;

public class UpdateCardRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
}

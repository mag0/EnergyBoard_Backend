
namespace EnergyBoard.Application.DTOs.request;

public class CreateColumnRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ProjectId { get; set; }
}

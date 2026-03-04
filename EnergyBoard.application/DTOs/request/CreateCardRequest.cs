
namespace EnergyBoard.Application.DTOs.request;

public class CreateCardRequest
{
    public string title { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
    public int ColumnId { get; set; }
}

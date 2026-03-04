
namespace EnergyBoard.Application.DTOs.request;

public class MoveCardRequest
{
    public int OriginalColumnId { get; set; }
    public int? NewColumnId { get; set; }
    public int NewPosition { get; set; }
}

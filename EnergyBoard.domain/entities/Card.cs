namespace EnergyBoard.Domain.entities;

public class Card : BaseEntity
{
    public DateTime Deadline { get; set; }
    public int ColumnId { get; set; }
    public Column? Column { get; set; }
}

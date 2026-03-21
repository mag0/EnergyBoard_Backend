namespace EnergyBoard.Domain.entities;

public class Column : BaseEntity
{
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public ICollection<Card> Cards { get; set; } = [];
}

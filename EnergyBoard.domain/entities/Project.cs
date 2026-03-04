namespace EnergyBoard.Domain.entities;

public class Project : BaseEntity
{
    public ICollection<Column> Columns { get; set; } = [];
}

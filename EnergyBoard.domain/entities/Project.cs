namespace EnergyBoard.Domain.entities;

public class Project : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public ICollection<Column> Columns { get; set; } = [];
}

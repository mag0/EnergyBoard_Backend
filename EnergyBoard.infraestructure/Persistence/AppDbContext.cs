using EnergyBoard.Domain.entities;
using Microsoft.EntityFrameworkCore;

namespace EnergyBoard.Infrastructure.Persistence;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Column> Columns => Set<Column>();
    public DbSet<Card> Cards => Set<Card>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>()
            .HasQueryFilter(p => !p.IsDeleted)
            .HasMany(p => p.Columns)
            .WithOne(c => c.Project)
            .HasForeignKey(c => c.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Column>()
            .HasQueryFilter(c => !c.IsDeleted)
            .HasMany(c => c.Cards)
            .WithOne(card => card.Column)
            .HasForeignKey(card => card.ColumnId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Card>()
            .HasQueryFilter(c => !c.IsDeleted);
    }
}
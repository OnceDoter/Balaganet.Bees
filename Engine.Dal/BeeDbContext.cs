using Engine.App;
using Engine.Field;
using Engine.Field.Events;
using Engine.Field.InternalEntities;
using Engine.Field.Map;
using Microsoft.EntityFrameworkCore;

namespace Engine.Dal;

public sealed class BeeDbContext : DbContext
{
    private static readonly DomainEventHandler DomainEventHandler = new();
    
    public BeeDbContext(DbContextOptions<BeeDbContext> options) : base(options)
    {
        Database.EnsureCreated();
        Database.Migrate();
    }
    
    public DbSet<InternalEntity> Entities { get; set; }
    public DbSet<State> States { get; set; }
    public DbSet<Dimension> Dimensions { get; set; }
    public DbSet<DimensionPart> DimensionParts { get; set; }
    
    public State State => States.First();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new EntityEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new StateEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new DimensionEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new DimensionPartEntityTypeConfiguration());
        modelBuilder.Entity<State>().HasData(new State());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        while (DomainEvent.Holder.Any(x => x.State == DomainEventState.Created))
        {
            foreach (var @event in DomainEvent.Holder.Where(x => x.State == DomainEventState.Created))
            {
                DomainEventHandler.Handle(@event);
            }
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }
}
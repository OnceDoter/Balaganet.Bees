using Engine.Field.Events;

namespace Engine.Field.Map;

public record Dimension : EntityBase
{
    public Dimension()
    {
        RaiseEvents(DomainEventType.DimensionCreated);
    }

    public int Id { get; set; }
    public virtual ICollection<DimensionPart> Parts { get; init; } = new HashSet<DimensionPart>();
}
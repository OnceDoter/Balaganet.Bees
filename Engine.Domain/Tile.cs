using Engine.Field.InternalEntities;

namespace Engine.Field;

public record Tile
{
    public required long X { get; init; }
    public required long Y { get; init; }
    
    public virtual ICollection<InternalEntity> Entities { get; init; } = new HashSet<InternalEntity>();
}
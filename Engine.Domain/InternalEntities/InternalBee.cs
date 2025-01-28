using Engine.Field.Common;

namespace Engine.Field.InternalEntities;

public record InternalBee : InternalEntity
{
    public override EntityType Type { get; init; } = EntityType.Bee;
    public Direction Direction { get; private set; }
    
    public virtual required ushort Speed { get; init; }
    public virtual required ushort Capacity { get; init; }

    public void Turn(Direction direction)
    {
        if (Direction != direction)
        {
            this.Direction = direction;
        }
        else
        {
            throw new Exception("Shouldn't be.");
        }
    }
}
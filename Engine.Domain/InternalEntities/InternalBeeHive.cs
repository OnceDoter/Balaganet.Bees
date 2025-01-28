namespace Engine.Field.InternalEntities;

public record InternalBeeHive : InternalBeeGroup
{
    public override EntityType Type { get; init; } = EntityType.BeeHive;
}
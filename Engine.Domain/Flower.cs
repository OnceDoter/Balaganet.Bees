namespace Engine.Field;

public record Flower : Resource
{
    public override EntityType Type { get; init; } = EntityType.Flower;
}
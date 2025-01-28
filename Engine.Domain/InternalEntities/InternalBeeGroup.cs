namespace Engine.Field.InternalEntities;

public record InternalBeeGroup : InternalBee
{
    public override EntityType Type { get; init; } = EntityType.BeeGroup;
    
    public virtual ICollection<InternalBee> Bees { get; init; } = null!;
    
    public required ushort HealthMultiplier { get; init; }
    public required ushort SpeedMultiplier { get; init; }
    public required ushort CapacityMultiplier { get; init; }

    public override required ushort Health
    {
        get => (ushort)(base.Health * HealthMultiplier);
        init => base.Health = value;
    }
    public override required ushort Speed
    {
        get => (ushort)(base.Speed * SpeedMultiplier);
        init => base.Speed = value;
    }
    public override required ushort Capacity
    {
        get => (ushort)(base.Capacity * CapacityMultiplier);
        init => base.Capacity = value;
    }
}
namespace Engine.Field.Map;

public record DimensionUnit(
    ushort X,
    ushort Y,
    string Metadata)
    : EntityBase;
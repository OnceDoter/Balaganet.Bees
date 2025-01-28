using Engine.Field.Map;

namespace Engine.App;

internal static class DimensionGenerator
{
    public static void GenerateDimension(Dimension dimension)
    {
        dimension.Parts.Add(new DimensionPart { Dimension = dimension });
    }
}
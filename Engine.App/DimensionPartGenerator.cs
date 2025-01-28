using System.Text.Json;
using Engine.Field.Map;

namespace Engine.App;

public class DimensionPartGenerator
{
    public static void GenerateDimensionPart(DimensionPart part)
    {
        for (ushort unitX = 0; unitX < DimensionPart.PartSize; unitX++)
        {
            for (ushort unitY = 0; unitY < DimensionPart.PartSize; unitY++)
            {
                var neighbourSoilType = part.GetUnitNeighbours(unitX, unitY)
                    .Select(unit => JsonSerializer.Deserialize<Dictionary<string, string>>(unit.Metadata)!)
                    .Where(metadata => metadata[nameof(UnitMetadata.SoilType)] != UnitMetadata.SoilType.Grass)
                    .Select(metadata => metadata[nameof(UnitMetadata.SoilType)])
                    .FirstOrDefault();

                var soilType = Random.Shared.Next(1, 100) switch
                {
                    <= 30 => Random.Shared.Next(1, 100) switch
                    {
                        > 70 => Random.Shared.Next(0, 12) switch
                        {
                            3 => UnitMetadata.SoilType.Fertile,
                            4 => UnitMetadata.SoilType.Rocky,
                            5 => UnitMetadata.SoilType.Sandy,
                            6 => UnitMetadata.SoilType.Clay,
                            7 => UnitMetadata.SoilType.Swamp,
                            8 => UnitMetadata.SoilType.Volcanic,
                            9 => UnitMetadata.SoilType.Magical,
                            10 => UnitMetadata.SoilType.Rainbow,
                            11 => UnitMetadata.SoilType.Polluted,
                            12 => UnitMetadata.SoilType.Technological,
                            _ => UnitMetadata.SoilType.Water
                        },
                        _ => UnitMetadata.SoilType.Grass
                    },
                    _ => neighbourSoilType ?? UnitMetadata.SoilType.Grass
                };

                part.Units.Add(new DimensionUnit(
                    unitX,
                    unitY,
                    JsonSerializer.Serialize(new Dictionary<string, string>()
                    {
                        { nameof(UnitMetadata.SoilType), soilType }
                    })));
            }
        }
    }
}
using Engine.Field.Events;
using Engine.Field.Map;

namespace Engine.Field.InternalEntities;

public abstract record InternalEntity : EntityBase
{
    public long Id { get; init; }
    public abstract EntityType Type { get; init; }
    public required ushort TileX { get; set; }
    public required ushort TileY { get; set; }
    public required long PartX { get; set; }
    public required long PartY { get; set; }
    public virtual DimensionPart Part { get; set; } = null!;
    public virtual DimensionUnit Unit => Part.Units.Single(t => t.X == TileX && t.Y == TileY);
    public virtual ICollection<Resource> BagPack { get; init; } = new HashSet<Resource>();
    public virtual required ushort Health { get; init; }
    public string? UserId { get; init; }

    public void UpdateCoordinates(short tileX, short tileY)
    {
        if (tileX < DimensionPart.PartSize && tileX >= 0)
        {
            TileX = (ushort)tileX;
        }
        else if (tileX == DimensionPart.PartSize)
        {
            if (Part.East is not null)
            {
                var chunk = Part.East;
                (PartX, PartY, Part) = (chunk.X, chunk.Y, chunk);
            }
            else
            {
                Part.Dimension.Parts.Add(new DimensionPart
                {
                    X = PartX + 1,
                    Y = TileY,
                    Dimension = Part.Dimension
                });
            }
        }
        else if (tileX == -1)
        {
            if (Part.West is not null)
            {
                var chunk = Part.West;
                (PartX, PartY, Part) = (chunk.X, chunk.Y, chunk);
            }
            else
            {
                Part.Dimension.Parts.Add(new DimensionPart
                {
                    X = PartX - 1,
                    Y = TileY,
                    Dimension = Part.Dimension
                });
            }
        }
        
        if (tileY < DimensionPart.PartSize && tileY >= 0)
        {
            TileY = (ushort)tileY;
        }
        else if (tileY == DimensionPart.PartSize)
        {
            if (Part.North is not null)
            {
                var chunk = Part.North;
                (PartX, PartY, Part) = (chunk.X, chunk.Y, chunk);
            }
            else
            {
                Part.Dimension.Parts.Add(new DimensionPart
                {
                    X = PartX,
                    Y = TileY + 1,
                    Dimension = Part.Dimension
                });
            }
        }
        else if (tileY == -1)
        {
            if (Part.South is not null)
            {
                var chunk = Part.South;
                (PartX, PartY, Part) = (chunk.X, chunk.Y, chunk);
            }
            else
            {
                Part.Dimension.Parts.Add(new DimensionPart
                {
                    X = PartX,
                    Y = TileY - 1,
                    Dimension = Part.Dimension
                });
            }
        }
    }
}
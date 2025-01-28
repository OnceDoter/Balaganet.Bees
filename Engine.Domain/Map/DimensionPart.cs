using Engine.Field.Events;

namespace Engine.Field.Map;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public record DimensionPart : EntityBase
{
    public const ushort PartSize = 1024;

    public DimensionPart()
    {
        RaiseEvents(DomainEventType.DimensionPartCreated);
    }
    
    public long X { get; init; }
    public long Y { get; init; }
    public int DimensionId { get; init; }

    public double Superficiality => Math.Sqrt(X * X + Y * Y);
    public Dimension Dimension { get; init; } = null!;

    public DimensionPart? West => Dimension.Parts.SingleOrDefault(x => x.X == X && x.Y == Y - 1);
    public DimensionPart? East => Dimension.Parts.SingleOrDefault(x => x.X == X && x.Y == Y + 1);
    public DimensionPart? South => Dimension.Parts.SingleOrDefault(x => x.X == X - 1 && x.Y == Y);
    public DimensionPart? North => Dimension.Parts.SingleOrDefault(x => x.X == X + 1 && x.Y == Y);
    
    public virtual ICollection<DimensionUnit> Units => new HashSet<DimensionUnit>();

    public IEnumerable<DimensionUnit> GetUnitNeighbours(ushort unitX, ushort unitY)
    {
        var north = this.GetNorthNeighbour(unitX, unitY);
        if (north is not null)
        {
            yield return north;
        }
        
        var south = this.GetSouthNeighbour(unitX, unitY);
        if (south is not null)
        {
            yield return south;
        }
        
        var west = this.GetWestNeighbour(unitX, unitY);
        if (west is not null)
        {
            yield return west;
        }
        
        var east = this.GetEastNeighbour(unitX, unitY);
        if (east is not null)
        {
            yield return east;
        }
    }

    private DimensionUnit? GetNorthNeighbour(ushort unitX, ushort unitY) 
        => unitX == 0 
            ? North?.Units.FirstOrDefault(unit => unit.X == 0 && unit.Y == unitY) 
            : Units.SingleOrDefault(unit => unit.X == unitX + 1 && unit.Y == unitY);

    private DimensionUnit? GetSouthNeighbour(ushort unitX, ushort unitY) 
        => unitX == 0 
            ? South?.Units.FirstOrDefault(unit => unit.X == PartSize - 1 && unit.Y == unitY) 
            : Units.SingleOrDefault(unit => unit.X == unitX - 1 && unit.Y == unitY);

    private DimensionUnit? GetEastNeighbour(ushort unitX, ushort unitY) 
        => unitX == 0 
            ? East?.Units.FirstOrDefault(unit => unit.X == unitX && unit.Y == 0) 
            : Units.SingleOrDefault(unit => unit.X == unitX  && unit.Y == unitY+ 1);

    private DimensionUnit? GetWestNeighbour(ushort unitX, ushort unitY) 
        => unitX == 0 
            ? West?.Units.FirstOrDefault(unit => unit.X == unitX && unit.Y == PartSize - 1) 
            : Units.SingleOrDefault(unit => unit.X == unitX && unit.Y == unitY- 1);
}
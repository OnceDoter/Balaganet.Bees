using Engine.Field;

namespace Blaganet.Bees.Engine.Models;

public class Chunk
{
    public long X { get; set; }
    public long Y { get; set; }
    public IEnumerable<Tile> Tiles { get; set; }
    public UserCompilation[] Compilations { get; set; }
}
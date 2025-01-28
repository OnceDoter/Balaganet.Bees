using Engine.Field;

namespace Blaganet.Bees.Engine.Models;

public sealed record ChunksBrief(ChunkBrief[] Chunks, State State);
public sealed record ChunkBrief(TileBrief[] Tiles);
public sealed record TileBrief(
    long X,
    long Y,
    EntityBrief[] Entities);
public sealed record EntityBrief(
    EntityType Type,
    long X,
    long Y);
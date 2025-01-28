using Blaganet.Bees.Engine.Models;
using Engine.Dal;
using Engine.Field;
using Engine.Field.InternalEntities;
using Engine.Field.Map;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tile = Engine.Field.Tile;

namespace Blaganet.Bees.Engine;

internal sealed partial class EngineFunctions
{
    [Function(nameof(GlobalIterationActivity))]
    public static async Task GlobalIterationActivity([ActivityTrigger] object foo, FunctionContext executionContext)
    {
        var beeContext = executionContext.InstanceServices.GetRequiredService<BeeDbContext>();
        (await beeContext.States.SingleAsync()).Invoke();
        await beeContext.SaveChangesAsync();
    }
    
    [Function(nameof(IterationActivity))]
    public static RaisedEvent[] IterationActivity([ActivityTrigger] Chunk context, FunctionContext executionContext)
    {
        return [];
    }
    
    [Function(nameof(StartCycleActivity))]
    public static async Task StartCycleActivity([ActivityTrigger] object foo, FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(StartCycleActivity));
        var beeContext = executionContext.InstanceServices.GetRequiredService<BeeDbContext>();
        logger.LogInformation("Starting cycle...");
        (await beeContext.States.SingleAsync()).Start();
        await beeContext.SaveChangesAsync();
        logger.LogInformation("Cycle started.");
    }
    
    [Function(nameof(CancelCycleActivity))]
    public static async Task CancelCycleActivity([ActivityTrigger] object foo, FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(CancelCycleActivity));
        var beeContext = executionContext.InstanceServices.GetRequiredService<BeeDbContext>();
        logger.LogInformation("Cancelling cycle...");
        (await beeContext.States.SingleAsync()).Stop();
        await beeContext.SaveChangesAsync();
        logger.LogInformation("Cycle cancelled.");
    }
    
    [Function(nameof(GetStateActivity))]
    public static async Task<State> GetStateActivity([ActivityTrigger] object foo, FunctionContext executionContext)
    {
        var beeContext = executionContext.InstanceServices.GetRequiredService<BeeDbContext>();
        return await beeContext.States.SingleAsync();
    }
    
    [Function(nameof(GetChunksActivity))]
    public static async Task<Chunk[]> GetChunksActivity([ActivityTrigger] int chunkSize, FunctionContext executionContext)
    {
        var beeContext = executionContext.InstanceServices.GetRequiredService<BeeDbContext>();
        return (await beeContext.Squares
            .GroupBy(tile => new
            {
                ChunkX = tile.X / chunkSize,
                ChunkY = tile.Y / chunkSize
            })
            .ToArrayAsync())
            .Select(group => new Chunk
            {
                X = group.Key.ChunkX,
                Y = group.Key.ChunkY,
                Tiles = group
            })
            .ToArray();
    }
    
    [Function(nameof(GetUserCompilationsActivity))]
    public static Task<UserCompilation[]> GetUserCompilationsActivity([ActivityTrigger] int batchSize, FunctionContext executionContext)
    {
        return Task.FromResult<UserCompilation[]>([
            new UserCompilation
            {
                UserId = "test0"
            }
        ]);
    }
    
    [Function(nameof(GetJuniorsActivity))]
    public static async Task<string[]> GetJuniorsActivity([ActivityTrigger] string[] userIds, FunctionContext executionContext)
    {
        var beeContext = executionContext.InstanceServices.GetRequiredService<BeeDbContext>();
        return
            userIds.Except(
                    await beeContext.Entities
                        .Where(x => x.UserId != null)
                        .Select(x => x.UserId!)
                        .ToArrayAsync())
                .ToArray();
    }
    
    [Function(nameof(GenerateChunkForJuniorActivity))]
    public static async Task GenerateChunkForJuniorActivity([ActivityTrigger] Junior june, FunctionContext executionContext)
    {
        june.ChunkSize = 32;
        june.UserId = "test0";
        
        var beeContext = executionContext.InstanceServices.GetRequiredService<BeeDbContext>();
        var dimension = await beeContext.Dimensions.OrderBy(x => x.Id).FirstAsync();

        if (!await beeContext.Squares.AnyAsync())
        {
            var (_, part) = DimensionPart.Ctor(dimension);
            beeContext.DimensionParts.Add(part);
            return;
        }

        var neighbourTileNum = Random.Shared.Next(-2, await beeContext.Squares.CountAsync() -1);
        if (neighbourTileNum < 0)
        {
            neighbourTileNum = 0;
        }
        
        var appendDirection = Random.Shared.Next(0, 3);
        var neighbourTile = await beeContext.Squares.Skip(neighbourTileNum).Take(1).SingleOrDefaultAsync()
                            ?? new Tile
                            {
                                X = 0,
                                Y = 0,
                            };
        var (chunkX, chunkY) = (neighbourTile.X / june.ChunkSize, neighbourTile.Y / june.ChunkSize);
        var (chunkLefTopX, chunkLefTopY) = (chunkX * june.ChunkSize, chunkY * june.ChunkSize);
        var neighbourLeftTopTileX = appendDirection switch
        {
            2 => await beeContext.Squares.Where(x => x.Y == chunkLefTopY).MinAsync(x => x.X),
            3 => await beeContext.Squares.Where(x => x.Y == chunkLefTopY).MaxAsync(x => x.X),
            _ => chunkLefTopX
        };
        var neighbourLeftTopTileY = appendDirection switch
        {
            0 => await beeContext.Squares.Where(x => x.X == chunkLefTopX).MinAsync(x => x.Y),
            1 => await beeContext.Squares.Where(x => x.X == chunkLefTopX).MaxAsync(x => x.Y),
            _ => chunkLefTopY
        };

        var (newLeftTopTileX, newLeftTopTileY) = appendDirection switch
        {
            0 => (chunkLefTopX, neighbourLeftTopTileY - june.ChunkSize),
            1 => (chunkLefTopX, neighbourLeftTopTileY + june.ChunkSize),
            2 => (neighbourLeftTopTileX - june.ChunkSize, chunkLefTopY),
            3 => (neighbourLeftTopTileX + june.ChunkSize, chunkLefTopY),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        var chunk2 = GenerateChunk(newLeftTopTileX, newLeftTopTileY, june.ChunkSize, june.UserId);
        await beeContext.Squares.AddRangeAsync(chunk2);
        await beeContext.SaveChangesAsync();
        
        return;

        static Tile[] GenerateChunk(long leftTopX, long leftTopY, int chunkSize, string userId)
        {
            var tiles = new List<Tile>(chunkSize * chunkSize);
            for (var x = leftTopX; x < chunkSize + leftTopX; x++)
            {
                for (var y = leftTopY; y < chunkSize + leftTopY; y++)
                {
                    tiles.Add(new Tile
                    {
                        X = x,
                        Y = y,
                    });
                }
            }
        
            var hiveNum = Random.Shared.Next(0, tiles.Count - 1);
            tiles[hiveNum].Entities.Add(new InternalBeeHive
            {
                PartX = tiles[hiveNum].X,
                PartY = tiles[hiveNum].Y,
                Health = 1,
                Speed = 1,
                Capacity = 10,
                CapacityMultiplier = 1,
                HealthMultiplier = 2,
                SpeedMultiplier = 3,
                UserId = userId
            });
            
            return tiles.ToArray();
        }
    }
}
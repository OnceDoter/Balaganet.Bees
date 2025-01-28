using System.Linq.Expressions;
using System.Net;
using Blaganet.Bees.Engine.Models;
using Engine.Dal;
using Engine.Field;
using LinqKit;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blaganet.Bees.Engine;

internal sealed partial class EngineFunctions
{
    [Function(nameof(HttpStart))]
    public async Task<HttpResponseData> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(HttpStart));

        return await ScheduleAndCreateCheckStatusResponseAsync(req, client, logger, nameof(StartCycle));
    }
    
    [Function(nameof(HttpStop))]
    public async Task<HttpResponseData> HttpStop(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(HttpStop));

        return await ScheduleAndCreateCheckStatusResponseAsync(req, client, logger, nameof(CancelCycle));
    }
    
    [Function(nameof(GetMine))]
    public async Task<HttpResponseData> GetMine(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var userId = req.Query["userId"];
        var beeContext = executionContext.InstanceServices.GetRequiredService<BeeDbContext>();
        var state = await beeContext.States.SingleAsync();
        var chunks = await beeContext.Entities
            .Where(x => x.UserId == userId)
            .Join(
                beeContext.Squares,
                x => new { X = x.PartX / ChunkSize, Y = x.PartY / ChunkSize },
                x => new { X = x.X / ChunkSize, Y = x.Y / ChunkSize },
                (entity, tile) => new { tile, entity })
            .GroupBy(x => new { X = x.tile.X / ChunkSize, Y = x.tile.Y / ChunkSize, IsXPositive = x.tile.X >= 0, IsYPositive = x.tile.Y >= 0 })
            .ToDictionaryAsync(x => x.Key, x => x.ToArray());
        
        var result = new ChunksBrief(
            chunks
                .Select(x => new ChunkBrief(
                    x.Value
                        .GroupBy(z => new { z.tile.X, z.tile.Y })
                        .Select(y => new TileBrief(y.Key.X, y.Key.Y, y
                            .Where(z => z.entity.PartX == y.Key.X && z.entity.PartY == y.Key.Y)
                            .Select(z => new EntityBrief(z.entity.Type, z.entity.PartX, z.entity.PartY)).ToArray()))
                        .ToArray()))
                .Where(x => x.Tiles.Length == ChunkSize * ChunkSize)
                .ToArray(), 
            state);
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(result);
        return response;
    }


    private static async Task<HttpResponseData> ScheduleAndCreateCheckStatusResponseAsync(
        HttpRequestData req,
        DurableTaskClient client,
        ILogger logger,
        string name)
    {
        if (await client.GetAllInstancesAsync(new OrchestrationQuery(Statuses: [OrchestrationRuntimeStatus.Pending], InstanceIdPrefix: name)).AnyAsync())
        {
            return await client.CreateCheckStatusResponseAsync(req, name);
        }
        
        await client.ScheduleNewOrchestrationInstanceAsync(name, CancellationToken.None);

        logger.LogInformation("Started orchestration {name} with ID = '{instanceId}'.", name, name);

        return await client.CreateCheckStatusResponseAsync(req, name);
    }
}
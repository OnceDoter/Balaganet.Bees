using Blaganet.Bees.Engine.Models;
using Engine.Field;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace Blaganet.Bees.Engine;

internal sealed partial class EngineFunctions
{
    private const int ChunkSize = 16;
    private const int ChunksToProcessSize = 1024;
    private const int UsersToProcessSize = 1024;
    
    [Function(nameof(StartCycle))]
    public static async Task StartCycle([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        await context.CallActivityAsync(nameof(StartCycleActivity));
        await context.CallSubOrchestratorAsync(nameof(Cycle));
    }
    
    [Function(nameof(CancelCycle))]
    public static Task CancelCycle([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        return context.CallActivityAsync(nameof(CancelCycleActivity));
    }
    
    [Function(nameof(Cycle))]
    public static async Task Cycle([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var logger = context.CreateReplaySafeLogger(nameof(StartCycle));
        var state = await context.CallActivityAsync<State>(nameof(GetStateActivity));

        logger.LogInformation("State: {State}.", state);
        if (!state.IsWorking)
        {
            logger.LogInformation("Cycle stopped.");
            return;
        }

        await context.CallActivityAsync<Chunk[]>(nameof(GlobalIterationActivity));

        
        var compilations = await context.CallActivityAsync<UserCompilation[]>(nameof(GetUserCompilationsActivity), UsersToProcessSize);
        var juniors = await context.CallActivityAsync<string[]>(nameof(GetJuniorsActivity), compilations.Select(x => x.UserId).ToArray());
        var chunks = await context.CallActivityAsync<Chunk[]>(nameof(GetChunksActivity), ChunksToProcessSize);

        await Task.WhenAll(
            Task.WhenAll(juniors.Select(june =>
                context.CallActivityAsync(nameof(GenerateChunkForJuniorActivity), new Junior{ ChunkSize = ChunkSize, UserId = june }))),
            Task.WhenAll(chunks.Select(chunk => context.CallActivityAsync(nameof(IterationActivity),
                new Chunk { X = chunk.X, Y = chunk.Y, Tiles = chunk.Tiles, Compilations = compilations })))
        );
        
        
        await context.CreateTimer(TimeSpan.FromSeconds(5), CancellationToken.None);
        context.ContinueAsNew();
    }
}
using Microsoft.Extensions.Logging;

namespace Desolate.Core.Scheduling;

/// <summary>
///     Implements the ability to for await to reschedule back into the scheduler.
/// </summary>
public sealed class RenderSynchronizationContext(IRenderScheduler renderScheduler, ILogger logger)
    : SynchronizationContext
{
    /// <summary>
    ///     Creates a copy of the synchronization context.
    /// </summary>
    public override SynchronizationContext CreateCopy()
    {
        return new RenderSynchronizationContext(renderScheduler, logger);
    }


    /// <summary>Dispatches an asynchronous message to a synchronization context.</summary>
    public override void Post(SendOrPostCallback d, object? state)
    {
        renderScheduler.ExecuteAsync(() =>
        {
            d(state);
            return Task.CompletedTask;
        }).ContinueWith(
            x => logger.LogError(x.Exception, "Error during synchronization context post."),
            TaskContinuationOptions.OnlyOnFaulted);
    }

    /// <summary>Dispatches a synchronous message to a synchronization context.</summary>
    public override void Send(SendOrPostCallback d, object? state)
    {
        renderScheduler.ExecuteAsync(() =>
        {
            d(state);
            return Task.CompletedTask;
        }).GetAwaiter().GetResult();
    }
}
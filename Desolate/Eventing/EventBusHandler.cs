namespace Desolate.Eventing;

/// <summary>
/// Adapts IEventHandler to be able to process predicates.
/// </summary>
public sealed class EventBusHandler<T>(Func<T, CancellationToken, ValueTask> handler) : IEventHandler<T>
{
    /// <inheritdoc />
    public async ValueTask Handle(T data, CancellationToken ct = default)
    {
        await handler(data, ct).ConfigureAwait(false);
    }
}
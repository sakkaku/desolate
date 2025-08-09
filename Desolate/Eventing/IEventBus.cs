namespace Desolate.Eventing;

/// <summary>
///     Event bus is used to dispatch objects in an async and thread safe manner.
/// </summary>
public interface IEventBus
{
    /// <summary>
    ///     Registers a handler.  An IDisposable token is returned that should be disposed when the event is no longer needed.
    /// </summary>
    IDisposable RegisterHandler<T>(IEventHandler<T> handler);

    /// <summary>
    /// Registers the specified predicate to handle an event.  An IDisposable token is returned that should be disposed when the event is no longer needed. 
    /// </summary>
    IDisposable RegisterHandler<T>(Func<T, CancellationToken, ValueTask> handler);

    /// <summary>
    ///     Raises an event.  The returned task completes after every handler has fired.
    /// </summary>
    ValueTask RaiseEvent<T>(T eventData, CancellationToken ct = default);

    /// <summary>
    ///     Raises an event after building the type with the passed in function.  The returned task completes after every
    ///     handler has fired.
    /// </summary>
    ValueTask BuildAndRaiseEvent<T>(Func<ValueTask<T>> buildEventData, CancellationToken ct = default);
}
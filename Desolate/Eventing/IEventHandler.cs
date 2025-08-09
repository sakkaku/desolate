namespace Desolate.Eventing;

/// <summary>
///     Implements an interface to handle a piece of data from the event bus.
/// </summary>
public interface IEventHandler<in T>
{
    /// <summary>
    ///     Handles the typed data from the event bus.
    /// </summary>
    ValueTask Handle(T data, CancellationToken ct = default);
}
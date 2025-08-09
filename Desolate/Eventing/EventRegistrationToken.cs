namespace Desolate.Eventing;

/// <summary>
/// IDisposable token used to free up event registrations
/// </summary>
public sealed class EventRegistrationToken(Action disposeAction) : IDisposable
{
    /// <inheritdoc />
    public void Dispose()
    {
        disposeAction();
    }
}
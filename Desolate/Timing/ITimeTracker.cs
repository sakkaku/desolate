namespace Desolate.Core.Timing;

/// <summary>
///     Provides information about the time state of the engine
/// </summary>
public interface ITimeTracker
{
    /// <summary>
    ///     Retrieves the current time as the duration since the simulation was started
    /// </summary>
    TimeSpan CurrentTime { get; }

    /// <summary>
    ///     The time since the last update
    /// </summary>
    TimeSpan DeltaTime { get; }

    /// <summary>
    ///     Resets the time tracker back to zero
    /// </summary>
    void Restart();

    /// <summary>
    ///     Applies an offset to the time tracking (e.g. to sync between sever and client)
    /// </summary>
    void SetOffset(TimeSpan offset);

    /// <summary>
    ///     Triggered at the start of a render to update the current and delta times
    /// </summary>
    public void UpdateTick();
}
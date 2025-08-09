namespace Desolate.Services;

/// <summary>
///     Provides information about the time state of the engine
/// </summary>
public interface ITimeTracker
{
    /// <summary>
    ///     Retrieves the current time as the duration since the simulation was started
    /// </summary>
    AbsoluteTime CurrentTime { get; }

    /// <summary>
    ///     The time since the last update
    /// </summary>
    DeltaTime DeltaTime { get; }

    /// <summary>
    ///     Resets the time tracker back to zero
    /// </summary>
    void Restart();

    /// <summary>
    ///     Applies an offset to the time tracking (e.g. to sync between sever and client)
    /// </summary>
    void SetOffset(DeltaTime offset);

    /// <summary>
    ///     Triggered at the start of a render to update the current and delta times
    /// </summary>
    public void UpdateTick();
}
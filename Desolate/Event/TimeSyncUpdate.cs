namespace Desolate.Event;

/// <summary>
///     Notifies that a new offset is to be used for time keeping
/// </summary>
public record TimeSyncUpdate(TimeSpan Offset);
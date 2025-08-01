using System.Diagnostics;
using Desolate.Core.Event;
using Desolate.Core.Eventing;

namespace Desolate.Core.Timing;

/// <summary>
///     Provides information about the time state of the engine
/// </summary>
public sealed class TimeTracker : ITimeTracker, IEventHandler<TimeSyncUpdate>, IDisposable
{
    /// <summary>
    ///     Initializes time tracker
    /// </summary>
    public TimeTracker(IEventBus eventBus)
    {
        _subTimeSync = eventBus.RegisterHandler(this);
    }

    private readonly IDisposable _subTimeSync;
    private readonly Stopwatch _absoluteWatch = new();
    private TimeSpan _offset = TimeSpan.Zero;
    private TimeSpan _lastAbsolute = TimeSpan.Zero;

    /// <inheritdoc />
    public TimeSpan CurrentTime { get; private set; }

    /// <inheritdoc />
    public TimeSpan DeltaTime { get; private set; }

    /// <inheritdoc />
    public void Restart()
    {
        _lastAbsolute = TimeSpan.Zero;
        _offset = TimeSpan.Zero;
        CurrentTime = TimeSpan.Zero;
        DeltaTime = TimeSpan.Zero;
        _absoluteWatch.Restart();
    }

    /// <inheritdoc />
    public void SetOffset(TimeSpan offset)
    {
        _offset = offset;
    }

    /// <inheritdoc />
    public void UpdateTick()
    {
        var absolute = _absoluteWatch.Elapsed;
        DeltaTime = absolute - _lastAbsolute;
        CurrentTime = absolute + _offset;
        _lastAbsolute = absolute;
    }

    /// <inheritdoc />
    public ValueTask Handle(TimeSyncUpdate data, CancellationToken ct = default)
    {
        SetOffset(data.Offset);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _subTimeSync.Dispose();
    }
}
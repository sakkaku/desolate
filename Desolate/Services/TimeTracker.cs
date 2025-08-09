using System.Diagnostics;
using Desolate.Event;
using Desolate.Eventing;

namespace Desolate.Services;

/// <summary>
///     Provides information about the time state of the engine
/// </summary>
public sealed class TimeTracker : ITimeTracker, IEventHandler<TimeSyncUpdate>, IDisposable
{
    private readonly Stopwatch _absoluteWatch = new();

    private readonly IDisposable _subTimeSync;
    private AbsoluteTime _lastAbsolute = TimeSpan.Zero;
    private DeltaTime _offset = TimeSpan.Zero;

    /// <summary>
    ///     Initializes time tracker
    /// </summary>
    public TimeTracker(IEventBus eventBus)
    {
        _subTimeSync = eventBus.RegisterHandler(this);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _subTimeSync.Dispose();
    }

    /// <inheritdoc />
    public ValueTask Handle(TimeSyncUpdate data, CancellationToken ct = default)
    {
        SetOffset(data.Offset);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public AbsoluteTime CurrentTime { get; private set; }

    /// <inheritdoc />
    public DeltaTime DeltaTime { get; private set; }

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
    public void SetOffset(DeltaTime offset)
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
}
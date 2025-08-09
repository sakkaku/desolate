using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace Desolate.Scheduling;

/// <inheritdoc />
public sealed class RenderScheduler : IRenderScheduler
{
    private readonly ILogger<RenderScheduler> _logger;
    private readonly ChannelReader<SchedulerQueueItem> _reader;
    private readonly ChannelWriter<SchedulerQueueItem> _writer;

    private bool _isShuttingDown;

    /// <summary>
    ///     Maintain a reference to the thread the scheduler is running on for tests.
    /// </summary>
    private Thread? _thread;

    /// <summary>
    ///     Initializes the render scheduler
    /// </summary>
    public RenderScheduler(ILogger<RenderScheduler> logger)
    {
        _logger = logger;

        var channel = Channel.CreateUnbounded<SchedulerQueueItem>(new UnboundedChannelOptions
        {
            AllowSynchronousContinuations = true,
            SingleReader = true,
            SingleWriter = false
        });

        _reader = channel.Reader;
        _writer = channel.Writer;
    }

    /// <inheritdoc />
    public Task ExecuteAsync(Func<Task> action)
    {
        var source = new TaskCompletionSource<Task>();

        try
        {
            if (ReferenceEquals(Thread.CurrentThread, _thread))
            {
                var task = action();
                source.SetResult(task);
            }
            else
            {
                _writer.TryWrite(new SchedulerQueueItem(action, source));
            }
        }
        catch (Exception ex)
        {
            source.SetException(ex);
        }

        return source.Task;
    }

    /// <inheritdoc />
    public void Run()
    {
        _thread = Thread.CurrentThread;

        SynchronizationContext.SetSynchronizationContext(new RenderSynchronizationContext(this, _logger));

        OnStartup?.Invoke();

        while (!_isShuttingDown)
        {
            if (!_reader.TryRead(out var item)) continue;

            try
            {
                var task = item.Action();
                item.Source.SetResult(task);
            }
            catch (Exception ex)
            {
                item.Source.SetException(ex);
            }
        }

        OnShutdown?.Invoke();
    }

    /// <inheritdoc />
    public Task Shutdown()
    {
        var source = new TaskCompletionSource<object?>();

        void SignalShutdownComplete()
        {
            source.SetResult(null);
            OnShutdown -= SignalShutdownComplete;
        }

        OnShutdown += SignalShutdownComplete;

        // handle case where shutdown already signaled
        if (Interlocked.Exchange(ref _isShuttingDown, true)) SignalShutdownComplete();

        return source.Task;
    }

    /// <inheritdoc />
    public event IRenderScheduler.HandleStartup? OnStartup;

    /// <inheritdoc />
    public event IRenderScheduler.HandleShutdown? OnShutdown;
}
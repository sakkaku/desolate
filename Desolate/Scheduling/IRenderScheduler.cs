namespace Desolate.Scheduling;

/// <summary>
///     Allows for the scheduling of tasks to run against the rendering thread.
/// </summary>
public interface IRenderScheduler
{
    /// <summary>
    ///     Delegate for shutdown event
    /// </summary>
    public delegate void HandleShutdown();

    /// <summary>
    ///     Delegate for start up event
    /// </summary>
    public delegate void HandleStartup();

    /// <summary>
    ///     Schedules an action against the render thread.
    /// </summary>
    Task ExecuteAsync(Func<Task> action);

    /// <summary>
    ///     Kicks off the loop where the promise will run until shutdown is called.
    /// </summary>
    void Run();

    /// <summary>
    ///     Notifies the queue that it should shut down.
    /// </summary>
    Task Shutdown();

    /// <summary>
    ///     Raised before the main event pump is started.
    /// </summary>
    event HandleStartup? OnStartup;

    /// <summary>
    ///     Raised after the event pump has stopped.
    /// </summary>
    event HandleShutdown? OnShutdown;
}
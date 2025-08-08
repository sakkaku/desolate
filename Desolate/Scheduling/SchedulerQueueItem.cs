namespace Desolate.Core.Scheduling;

/// <summary>
///     Contains an item waiting to be scheduled along with a completion source to update.
/// </summary>
public sealed record SchedulerQueueItem(Func<Task> Action, TaskCompletionSource<Task> Source);
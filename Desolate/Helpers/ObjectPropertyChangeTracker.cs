using System.ComponentModel;

namespace Desolate.Helpers;

/// <summary>
///     Implements a mechanism to detect when changes happen to objects tracked by INotifyPropertyChanged
/// </summary>
public sealed class ObjectPropertyChangeTracker<T> : IDisposable where T : INotifyPropertyChanged
{
    private readonly List<T> _subscriptions = [];
    private HashSet<ChangedProperty<T>> _updates = [];

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var target in _subscriptions) target.PropertyChanged -= TargetOnPropertyChanged;

        _subscriptions.Clear();
    }

    /// <summary>
    ///     Adds an object to the change tracker.
    /// </summary>
    public void Track(T target)
    {
        target.PropertyChanged += TargetOnPropertyChanged;
        _subscriptions.Add(target);
    }

    /// <summary>
    ///     Removes an object from the change tracker.
    /// </summary>
    public void Untrack(T target)
    {
        target.PropertyChanged -= TargetOnPropertyChanged;
        _subscriptions.Remove(target);
        _updates.RemoveWhere(x => ReferenceEquals(x.Target, target));
    }

    /// <summary>
    ///     Returns true if changes have been detected.
    /// </summary>
    public bool HasUpdates()
    {
        return _updates.Count > 0;
    }

    /// <summary>
    ///     Retrieves the current changes and resets the tracker.
    /// </summary>
    public HashSet<ChangedProperty<T>> GetUpdatesAndReset()
    {
        var updates = _updates;
        _updates = [];
        return updates;
    }

    private void TargetOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is T target && e.PropertyName is not null)
            _updates.Add(new ChangedProperty<T>(target, e.PropertyName));
    }

    /// <summary>
    ///     Represents a property that has changed in the target.
    /// </summary>
    public record ChangedProperty<T>(T Target, string PropertyName);
}
namespace Desolate.Eventing;

/// <summary>
/// Implements an async event bus that uses IDisposable registrations.
/// </summary>
public sealed class EventBus : IEventBus, IDisposable
{
    private readonly Dictionary<Type, List<object>> _handlers = new();
    private readonly ReaderWriterLockSlim _lock = new();

    /// <inheritdoc />
    public void Dispose()
    {
        _lock.Dispose();
    }

    /// <inheritdoc />
    public IDisposable RegisterHandler<T>(IEventHandler<T> handler)
    {
        _lock.EnterWriteLock();

        try
        {
            var type = typeof(T);

            if (!_handlers.TryGetValue(type, out var registrations))
            {
                registrations = new List<object>();
                _handlers.Add(type, registrations);
            }

            var token = new EventRegistrationToken(() => { RemoveRegistration(type, registrations, handler); });

            registrations.Add(handler);

            return token;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc />
    public IDisposable RegisterHandler<T>(Func<T, CancellationToken, ValueTask> handler)
    {
        return RegisterHandler(new EventBusHandler<T>(handler));
    }

    /// <inheritdoc />
    public async ValueTask RaiseEvent<T>(T eventData, CancellationToken ct = default)
    {
        _lock.EnterReadLock();

        try
        {
            if (!_handlers.TryGetValue(typeof(T), out var registrations)) return;

            foreach (IEventHandler<T> handler in registrations)
            {
                if (ct.IsCancellationRequested) break;

                await handler.Handle(eventData, ct).ConfigureAwait(false);
            }
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <inheritdoc />
    public async ValueTask BuildAndRaiseEvent<T>(Func<ValueTask<T>> buildEventData, CancellationToken ct = default)
    {
        _lock.EnterReadLock();

        try
        {
            if (!_handlers.TryGetValue(typeof(T), out var registrations)) return;

            if (ct.IsCancellationRequested) return;

            var eventData = await buildEventData().ConfigureAwait(false);

            foreach (IEventHandler<T> handler in registrations)
            {
                if (ct.IsCancellationRequested) break;

                await handler.Handle(eventData, ct).ConfigureAwait(false);
            }

            switch (eventData)
            {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                    break;
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    private void RemoveRegistration(Type type, ICollection<object> registrations, object handler)
    {
        _lock.EnterWriteLock();

        try
        {
            registrations.Remove(handler);

            if (registrations.Count <= 0) _handlers.Remove(type);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }
}
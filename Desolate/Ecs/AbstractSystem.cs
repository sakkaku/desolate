using CommunityToolkit.Diagnostics;
using Desolate.Event;
using Desolate.Eventing;

namespace Desolate.Ecs;

/// <summary>
/// Implements the generic functionality required for entity tracking in a system.
/// </summary>
public abstract class AbstractSystem : IEcsSystem, IDisposable,
    IEventHandler<EntityRemoved>,
    IEventHandler<EntityComponentAdded>,
    IEventHandler<EntityComponentRemoved>
{
    private readonly IReadOnlyList<IDisposable> _events;
    private readonly List<Entity> _trackedEntities = new();

    /// <summary>
    ///     The component types that this system is interested in registering.
    /// </summary>
    protected readonly HashSet<Type> ComponentTypes = new();

    /// <summary>
    /// The entities that are currently tracked by this system.
    /// </summary>
    protected readonly IReadOnlyList<Entity> TrackedEntities;

    /// <summary>
    /// Initializes the abstract system.
    /// </summary>
    protected AbstractSystem(IEventBus bus)
    {
        _events =
        [
            bus.RegisterHandler<EntityRemoved>(this),
            bus.RegisterHandler<EntityComponentAdded>(this),
            bus.RegisterHandler<EntityComponentRemoved>(this),
        ];

        TrackedEntities = _trackedEntities;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }


    /// <inheritdoc />
    public World? World { get; set; }

    public abstract ValueTask Update(CancellationToken ct);

    /// <inheritdoc />
    public async ValueTask Handle(EntityComponentAdded data, CancellationToken ct = default)
    {
        if (ComponentTypes.Contains(data.ComponentType))
        {
            Guard.IsNotNull(World);

            foreach (var type in ComponentTypes)
            {
                if (!World.HasComponent(data.Entity, type))
                {
                    return;
                }
            }

            if (!await ValidateEntity(data.Entity, ct))
            {
                return;
            }

            _trackedEntities.Add(data.Entity);
            await OnEntityAdded(data.Entity, ct);
        }
    }

    /// <inheritdoc />
    public async ValueTask Handle(EntityComponentRemoved data, CancellationToken ct = default)
    {
        if (ComponentTypes.Contains(data.ComponentType))
        {
            _trackedEntities.Remove(data.Entity);
            await OnEntityRemoved(data.Entity, ct);
        }
    }

    /// <inheritdoc />
    public ValueTask Handle(EntityRemoved data, CancellationToken ct = default)
    {
        _trackedEntities.Remove(data.Entity);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Allows for the entity to be veto'd from addition to the system.
    /// </summary>
    protected virtual ValueTask<bool> ValidateEntity(Entity entity, CancellationToken ct)
    {
        return ValueTask.FromResult(true);
    }

    /// <summary>
    /// Called when the entity is removed from the system.
    /// </summary>
    protected virtual ValueTask OnEntityRemoved(Entity entity, CancellationToken ct)
    {
        return default;
    }

    /// <summary>
    /// Called when the entity is added to the system.
    /// </summary>
    protected virtual ValueTask OnEntityAdded(Entity entity, CancellationToken ct)
    {
        return default;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (var registration in _events) registration.Dispose();
        }
    }
}
using Desolate.Event;
using Desolate.Eventing;
using Microsoft.Extensions.DependencyInjection;

namespace Desolate.Ecs;

/// <summary>
///     Represents the combined state of entities, components and systems.
/// </summary>
public sealed class World(IServiceProvider services, IEventBus eventBus) : IDisposable
{
    private readonly Dictionary<int, Entity> _entities = [];
    private readonly EntityKeyManager _keyManager = new();
    private readonly List<IEcsSystem> _systems = [];

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var system in _systems.OfType<IDisposable>()) system.Dispose();
    }

    /// <summary>
    ///     Creates a new entity within the world.
    /// </summary>
    /// <param name="name">A human-readable name for the entity</param>
    public async ValueTask<Entity> CreateEntity(string name)
    {
        var entity = new Entity { Id = _keyManager.GetId(), Name = name };
        _entities.Add(entity.Id, entity);
        await eventBus.BuildAndRaiseEvent(() => ValueTask.FromResult(new EntityAdded(entity)));
        return entity;
    }

    /// <summary>
    ///     Removes the entity from the world.
    /// </summary>
    public async ValueTask RemoveEntity(Entity entity)
    {
        _entities.Remove(entity.Id);
        _keyManager.ReleaseId(entity.Id);
        await eventBus.BuildAndRaiseEvent(() => ValueTask.FromResult(new EntityRemoved(entity)));
    }

    /// <summary>
    ///     Retrieves the entity with the given identifier.
    /// </summary>
    public ValueTask<Entity?> GetEntity(int id)
    {
        if (_entities.TryGetValue(id, out var entity)) return ValueTask.FromResult<Entity?>(entity);

        return ValueTask.FromResult<Entity?>(null);
    }

    /// <summary>
    ///     Adds a component to the entity.
    /// </summary>
    public async ValueTask AddComponent<T>(Entity entity, T component) where T : notnull, IEcsComponent
    {
        var type = typeof(T);

        entity.AttachComponent(typeof(T), component);

        await eventBus.BuildAndRaiseEvent(()
            => ValueTask.FromResult(new EntityComponentAdded(entity, type, component)));
    }

    /// <summary>
    ///     Removes the component from the entity.
    /// </summary>
    public async ValueTask RemoveComponent<T>(Entity entity) where T : IEcsComponent
    {
        var type = typeof(T);

        if (entity.DetachComponent<T>(out var removed))
            await eventBus.BuildAndRaiseEvent(()
                => ValueTask.FromResult(new EntityComponentRemoved(entity, type, removed)));
    }

    /// <summary>
    ///     Registers the system via dependency injection
    /// </summary>
    public async ValueTask RegisterSystem<T>() where T : IEcsSystem
    {
        var system = services.GetRequiredService<T>();
        await RegisterSystem(system);
    }

    /// <summary>
    ///     Registers the system into the world.
    /// </summary>
    public ValueTask RegisterSystem<T>(T system) where T : IEcsSystem
    {
        system.World = this;
        _systems.Add(system);
        return default;
    }

    /// <summary>
    ///     Removes the systems from the simulation
    /// </summary>
    public ValueTask UnregisterSystem<T>(T system) where T : IEcsSystem
    {
        system.World = null;

        _systems.Remove(system);

        if (system is IDisposable disposable) disposable.Dispose();

        return default;
    }

    /// <summary>
    ///     Retrieves the systems that match the specified type.
    /// </summary>
    public IEnumerable<T> GetSystems<T>() where T : IEcsSystem
    {
        return _systems.OfType<T>();
    }

    /// <summary>
    ///     Updates the world.
    /// </summary>
    public async ValueTask Update(CancellationToken ct)
    {
        foreach (var system in _systems) await system.Update(ct);
    }
}
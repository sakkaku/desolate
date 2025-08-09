using CommunityToolkit.Diagnostics;
using Desolate.Core.Eventing;
using Desolate.Event;
using Microsoft.Extensions.DependencyInjection;

namespace Desolate.Ecs;

public sealed class World (IServiceProvider services, IEventBus eventBus) : IDisposable
{
    private readonly EntityKeyManager _keyManager = new();
    private readonly Dictionary<int, Entity> _entities = new();
    private readonly Dictionary<Type, Dictionary<int, object>> _components = new();
    private readonly Dictionary<Type, System> _systems = new ();

    public async ValueTask<Entity> CreateEntity(string name)
    {
        var entity = new Entity(_keyManager.GetId(), name);
        _entities.Add(entity.Id, entity);
        await eventBus.RaiseEvent(new EntityAdded(entity));
        return entity;
    }
    
    public async ValueTask RemoveEntity(Entity entity)
    {
        foreach (var components in _components.Values)
        {
            components.Remove(entity.Id);
        }
        _entities.Remove(entity.Id);
        _keyManager.ReleaseId(entity.Id);
        await eventBus.RaiseEvent(new EntityRemoved(entity));
    }
    
    public async ValueTask AddComponent<T>(Entity entity, T component) where T : notnull
    {
        var type = typeof(T);

        if (!_components.TryGetValue(type, out var components))
        {
            components = new Dictionary<int, object>();
            _components.Add(type, components);
        }

        components.Add(entity.Id, component);
        await eventBus.RaiseEvent(new EntityComponentAdded(entity, type, component));
    }
    
    public async ValueTask RemoveComponent<T>(Entity entity)
    {
        var type = typeof(T);
        
        if (_components.TryGetValue(typeof(T), out var components))
        {
            if (components.Remove(entity.Id, out var removed))
            {
                await eventBus.RaiseEvent(new EntityComponentAdded(entity, type, removed));
            }
        }
    }
    
    public T GetComponent<T>(Entity entity)
    {
        if (_components.TryGetValue(typeof(T), out var components) && 
            components.TryGetValue(entity.Id, out var component))
        {
            return (T) component;
        }

        return default;
    }
    
    public bool HasComponent<T>(Entity entity)
    {
        return _components.TryGetValue(typeof(T), out var components) 
               && components.ContainsKey(entity.Id);
    }
    
    public ValueTask RegisterSystem<T>() where T : System
    {
        var system = services.GetRequiredService<T>();
        _systems.Add(typeof(T), system);
        return default;
    }
    
    public T GetSystem<T>() where T : System
    {
        Guard.IsTrue(_systems.TryGetValue(typeof(T), out var system));
        return (T) system;
    }
    
    public async ValueTask Update(CancellationToken ct)
    {
        foreach (var (_, system) in _systems)
        {
            await system.Update(ct);
        }
    }

    public void Dispose()
    {
        foreach (var system in _systems.Values)
        {
            system.Dispose();
        }
    }
}
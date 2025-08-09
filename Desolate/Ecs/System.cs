using Desolate.Core.Eventing;
using Desolate.Event;

namespace Desolate.Ecs;

public abstract class System : IDisposable
{
    private readonly IReadOnlyList<IDisposable> _events;
    protected readonly IReadOnlyList<Entity> TrackedEntities;
    
    protected System(IEventBus bus, HashSet<Type> componentTypes)
    {
        var tracked = new List<Entity>();
        
        _events = new List<IDisposable>
        {
            bus.RegisterHandler<EntityRemoved>((removed, ct) =>
            {
                tracked.Remove(removed.Entity);
                return ValueTask.CompletedTask;
            }),
            bus.RegisterHandler<EntityComponentAdded>((added, ct) =>
            {
                
            })
        };

        TrackedEntities = tracked;
    }
    
    public abstract ValueTask Update(CancellationToken ct);

    public void Dispose()
    {
        foreach (var registration in _events)
        {
            registration.Dispose();
        }
    }
}
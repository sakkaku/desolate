using CommunityToolkit.Diagnostics;
using Desolate.Components;
using Desolate.Ecs;
using Desolate.Eventing;
using Desolate.Services;

namespace Desolate.Systems;

/// <summary>
///     Implements a system to expire objects after absolute time has passed.
/// </summary>
public sealed class TemporaryObjectSystem : AbstractSystem
{
    private readonly ITimeTracker _tracker;

    /// <inheritdoc />
    public TemporaryObjectSystem(IEventBus bus, ITimeTracker tracker) : base(bus)
    {
        _tracker = tracker;
        ComponentTypes.Add(typeof(TemporaryObject));
    }

    public override async ValueTask Update(CancellationToken ct)
    {
        Guard.IsNotNull(World);

        var toRemove = new List<Entity>();

        foreach (var entity in TrackedEntities)
        {
            var expiration = World.GetComponent<TemporaryObject>(entity);

            if (expiration?.Expiration < _tracker.CurrentTime) toRemove.Add(entity);
        }

        foreach (var entity in toRemove) await World.RemoveEntity(entity);
    }
}
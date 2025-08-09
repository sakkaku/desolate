using CommunityToolkit.Diagnostics;
using Desolate.Components;
using Desolate.Ecs;
using Desolate.Eventing;
using Desolate.Services;

namespace Desolate.Systems;

/// <summary>
///     Implements a naive system to interpolate physics between frames on the client.
/// </summary>
public sealed class InterpolatingPhysicsSystem : AbstractSystem
{
    private readonly ITimeTracker _time;

    /// <summary>
    ///     Initializes the physics system
    /// </summary>
    public InterpolatingPhysicsSystem(IEventBus bus, ITimeTracker time) : base(bus)
    {
        _time = time;

        ComponentTypes.Add(typeof(PositionedObject));
        ComponentTypes.Add(typeof(PhysicalObject));
    }

    /// <inheritdoc />
    public override ValueTask Update(CancellationToken ct)
    {
        Guard.IsNotNull(World);

        foreach (var entity in TrackedEntities)
        {
            var physics = World.GetComponent<PhysicalObject>(entity);

            Guard.IsNotNull(physics);

            if (physics.AngularVelocity != default || physics.PositionalVelocity != default) continue;

            var position = World.GetComponent<PositionedObject>(entity);

            Guard.IsNotNull(position);

            var delta = _time.DeltaTime;

            position.Position += physics.PositionalVelocity * delta;
            position.Rotation += physics.AngularVelocity * delta;
        }

        return default;
    }
}
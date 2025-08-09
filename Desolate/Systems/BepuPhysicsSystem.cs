using Desolate.Components;
using Desolate.Ecs;
using Desolate.Eventing;

namespace Desolate.Systems;

/// <summary>
///     Implements physics using bepu
/// </summary>
public sealed class BepuPhysicsSystem : AbstractSystem
{
    /// <summary>
    ///     Initializes the physics system
    /// </summary>
    public BepuPhysicsSystem(IEventBus bus) : base(bus)
    {
        ComponentTypes.Add(typeof(PositionedObject));
        ComponentTypes.Add(typeof(PhysicalObject));
    }

    public override ValueTask Update(CancellationToken ct)
    {
        return default;
    }
}
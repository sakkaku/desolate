using Desolate.Components;
using Desolate.Ecs;
using Desolate.Eventing;

namespace Desolate.Systems;

/// <summary>
///     Keeps a hierarchical object offset from the parent.
/// </summary>
public sealed class ParentOffsetSystem : AbstractSystem
{
    /// <summary>
    /// Initializes the system.
    /// </summary>
    public ParentOffsetSystem(IEventBus bus) : base(bus)
    {
        RequiredComponentTypes.Add(typeof(HierarchyParent));
        RequiredComponentTypes.Add(typeof(HierarchyOffset));
        RequiredComponentTypes.Add(typeof(PositionedObject));
    }

    /// <inheritdoc />
    public override ValueTask Update(CancellationToken ct)
    {
        foreach (var entity in TrackedEntities)
        {
            var position = entity.GetComponent<PositionedObject>();
            var offset = entity.GetComponent<HierarchyOffset>();
            var parentEntity = entity.GetComponent<HierarchyParent>().Parent;
            var parentPosition = parentEntity.GetComponent<PositionedObject>();

            position.Position = parentPosition.Position + offset.DeltaPosition;
            position.Rotation = parentPosition.Rotation * offset.DeltaRotation;
        }

        return default;
    }
}
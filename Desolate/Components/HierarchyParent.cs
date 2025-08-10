using Desolate.Ecs;

namespace Desolate.Components;

/// <summary>
///     Assigns a parent entity.
/// </summary>
public sealed record HierarchyParent(Entity Parent) : IEcsComponent;
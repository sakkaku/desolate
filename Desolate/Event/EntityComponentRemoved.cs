using Desolate.Ecs;

namespace Desolate.Event;

/// <summary>
/// Raised when a component is removed from the entity.
/// </summary>
public record EntityComponentRemoved(Entity Entity, Type ComponentType, object Component);
using Desolate.Ecs;

namespace Desolate.Event;

/// <summary>
///     Raised when a component is added to an entity.
/// </summary>
public record EntityComponentAdded(Entity Entity, Type ComponentType, object Component);
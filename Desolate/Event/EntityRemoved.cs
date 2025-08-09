using Desolate.Ecs;

namespace Desolate.Event;

/// <summary>
/// Raised when an entity is removed from the world.
/// </summary>
public record EntityRemoved(Entity Entity);
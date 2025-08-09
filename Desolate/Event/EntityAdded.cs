using Desolate.Ecs;

namespace Desolate.Event;

/// <summary>
/// Raised when an entity is added to the world.
/// </summary>
public record EntityAdded(Entity Entity);
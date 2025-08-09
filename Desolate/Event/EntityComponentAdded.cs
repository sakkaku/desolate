using Desolate.Ecs;

namespace Desolate.Event;

public record EntityComponentAdded(Entity Entity, Type ComponentType, object Component);
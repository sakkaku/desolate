using Desolate.Ecs;

namespace Desolate.Event;

public record EntityComponentRemoved(Entity Entity, Type ComponentType, object Component);
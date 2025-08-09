using Desolate.Ecs;

namespace Desolate.Event;

public record EntityComponentRemoved(Entity Entity, type ComponentType, object Component);
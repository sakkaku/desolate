using Desolate.Ecs;

namespace Desolate.Components;

/// <summary>
/// Flags that an object will be removed automatically at the expiration.
/// </summary>
public sealed record TemporaryObject(AbsoluteTime Expiration) : IEcsComponent;
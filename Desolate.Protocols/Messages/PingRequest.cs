using MemoryPack;

namespace Desolate.Protocols.Messages;

/// <summary>
///     Contains a ping request that should be responded with a pong response with the same Id.
/// </summary>
[MemoryPackable]
public partial record PingRequest(Guid Id, DateTimeOffset Timestamp);
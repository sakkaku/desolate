using MemoryPack;

namespace Desolate.Protocols.Messages;

/// <summary>
///     A response to a ping request.  The Id should match the ping request.
/// </summary>
[MemoryPackable]
public partial record PongResponse(Guid Id, DateTimeOffset Timestamp);
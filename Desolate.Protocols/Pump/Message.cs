namespace Desolate.Protocols.Pump;

/// <summary>
///     Wrapper class for messages.
/// </summary>
public record Message(MessageType Type, Memory<byte> Data);
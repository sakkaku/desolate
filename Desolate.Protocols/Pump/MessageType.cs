namespace Desolate.Protocols.Pump;

/// <summary>
///     Determination for what type is contained in a message
/// </summary>
public enum MessageType
{
    /// <summary>
    ///     Default type used to detect when conversion fails
    /// </summary>
    Unknown = 0,

    Ping = 1,

    Pong = 2
}
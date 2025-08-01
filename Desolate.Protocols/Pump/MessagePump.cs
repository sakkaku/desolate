using System.Buffers;
using System.Buffers.Binary;
using CommunityToolkit.Diagnostics;

namespace Desolate.Protocols.Pump;

/// <summary>
///     Utility class to handle sending and processing messages.
/// </summary>
public sealed class MessagePump : IDisposable
{
    private readonly CancellationTokenSource _cts = new();
    private readonly Stream _stream;
    private readonly SemaphoreSlim _writeSemaphore = new(1, 1);

    private const int MaxMessageSize = 0x800000; // 8 MiB

    /// <summary>
    ///     Initializes the message pump and takes ownership of a stream.
    /// </summary>
    public MessagePump(Stream stream)
    {
        _stream = stream;
    }

    /// <summary>
    ///     Event that will be raised when a message is received.
    /// </summary>
    public Func<Message, Task>? OnMessage { get; init; }

    /// <summary>
    ///     Event that will be raised when socket is disconnected.
    /// </summary>
    public Func<Task>? OnDisconnect { get; init; }

    /// <inheritdoc />
    public void Dispose()
    {
        _stream.Dispose();
        _cts.Dispose();
        _writeSemaphore.Dispose();
    }

    /// <summary>
    ///     Writes the given message to the underlying stream.
    ///     This method is thread safe.
    /// </summary>
    public async Task WriteAsync(Message message)
    {
        ValidateMessageSize(message.Data.Length);
        ValidateMessageType(message.Type);

        var ct = _cts.Token;
        await _writeSemaphore.WaitAsync(ct).ConfigureAwait(false);

        try
        {
            var headerSize = sizeof(MessageType) + sizeof(int);
            var headerBuffer = ArrayPool<byte>.Shared.Rent(headerSize);
            var header = new Memory<byte>(headerBuffer, 0, headerSize);

            try
            {
                BinaryPrimitives.WriteInt32LittleEndian(header.Slice(0, sizeof(MessageType)).Span, (int)message.Type);
                BinaryPrimitives.WriteInt32LittleEndian(header.Slice(sizeof(MessageType), sizeof(int)).Span,
                    message.Data.Length);

                await _stream.WriteAsync(header, ct).ConfigureAwait(false);
                await _stream.WriteAsync(message.Data, ct).ConfigureAwait(false);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(headerBuffer);
            }
        }
        finally
        {
            _writeSemaphore.Release();
        }
    }

    /// <summary>
    ///     Initializes processing of messages.
    /// </summary>
    public void Start()
    {
        Task.Run(() => ConsumeStream(_cts.Token));
    }

    private async Task ConsumeStream(CancellationToken ct)
    {
        var headerBuffer = new byte[sizeof(uint) + sizeof(MessageType)];
        var headers = new Memory<byte>(headerBuffer);

        try
        {
            while (!ct.IsCancellationRequested)
            {
                await _stream.ReadExactlyAsync(headers, ct).ConfigureAwait(false);

                var type = (MessageType)BinaryPrimitives.ReadInt32LittleEndian(headers.Slice(0, sizeof(MessageType))
                    .Span);
                ValidateMessageType(type);
                var size = BinaryPrimitives.ReadInt32LittleEndian(headers.Slice(sizeof(MessageType), sizeof(int)).Span);
                ValidateMessageSize(size);

                var dataBuffer = ArrayPool<byte>.Shared.Rent(size);
                try
                {
                    await _stream.ReadExactlyAsync(dataBuffer, ct).ConfigureAwait(false);
                    var message = new Message(type, dataBuffer);
                    await (OnMessage?.Invoke(message) ?? Task.CompletedTask);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(dataBuffer);
                }
            }
        }
        finally
        {
            await (OnDisconnect?.Invoke() ?? Task.CompletedTask);
        }
    }

    private static void ValidateMessageSize(int size)
    {
        Guard.IsBetween(size, 0, MaxMessageSize);
    }

    private static void ValidateMessageType(MessageType type)
    {
        if (type == MessageType.Unknown) throw new InvalidOperationException("Message type is unknown");
    }

    /// <summary>
    ///     Stops the processing of messages.
    /// </summary>
    public void Stop()
    {
        _cts.Cancel();
    }
}
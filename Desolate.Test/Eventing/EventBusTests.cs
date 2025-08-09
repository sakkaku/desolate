using Desolate.Eventing;

namespace Desolate.Test.Eventing;

[TestClass]
public sealed class EventBusTests
{
    [TestMethod]
    public async Task RegisterAndReceiveEvent()
    {
        using var bus = new EventBus();
        string? result = null;
        using var handle = bus.RegisterHandler<string>((x, ct) =>
        {
            result = x;
            return ValueTask.CompletedTask;
        });
        await bus.RaiseEvent("Hello World");
        Assert.AreEqual("Hello World", result);
    }

    [TestMethod]
    public async Task RaiseEventWithNoHandlers()
    {
        using var bus = new EventBus();
        await bus.RaiseEvent("Hello World");
    }

    [TestMethod]
    public async Task RaiseEventWithCollector()
    {
        using var bus = new EventBus();
        int? result = null;
        using var handle = bus.RegisterHandler<int>((x, ct) =>
        {
            result = x;
            return ValueTask.CompletedTask;
        });
        await bus.BuildAndRaiseEvent(() => ValueTask.FromResult(123));
        Assert.AreEqual(123, result);
    }

    [TestMethod]
    public async Task RaiseEventWorksWithComplexTypes()
    {
        using var bus = new EventBus();
        var handler = new ExampleHandler();
        using var handle = bus.RegisterHandler(handler);
        ExampleEvent? eventData = null;
        await bus.BuildAndRaiseEvent(() =>
        {
            eventData = new ExampleEvent { Value = 123 };
            return ValueTask.FromResult(eventData);
        });
        Assert.IsNotNull(eventData);
        Assert.IsTrue(eventData.Disposed);
        Assert.IsTrue(handler.IsSuccess);
    }

    private class ExampleEvent : IAsyncDisposable
    {
        public int Value { get; init; }

        public bool Disposed { get; private set; }

        public ValueTask DisposeAsync()
        {
            Disposed = true;
            return ValueTask.CompletedTask;
        }
    }

    private class ExampleHandler : IEventHandler<ExampleEvent>
    {
        public bool IsSuccess { get; private set; }

        public ValueTask Handle(ExampleEvent data, CancellationToken ct = default)
        {
            IsSuccess = data.Value == 123;
            return ValueTask.CompletedTask;
        }
    }
}
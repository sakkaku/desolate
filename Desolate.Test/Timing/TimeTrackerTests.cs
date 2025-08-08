using Desolate.Core.Eventing;
using Desolate.Core.Timing;
using Desolate.Timing;

namespace Desolate.Test.Timing;

[TestClass]
public sealed class TimeTrackerTests
{
    private readonly EventBus _eventBus = new();

    [TestMethod]
    public async Task DoesNotAdvanceUntilReset()
    {
        var tracker = new TimeTracker(_eventBus);
        await Task.Delay(1);
        tracker.UpdateTick();
        Assert.AreEqual(TimeSpan.Zero, tracker.DeltaTime);
        Assert.AreEqual(TimeSpan.Zero, tracker.CurrentTime);
    }

    [TestMethod]
    public async Task AdvancesAfterReset()
    {
        var tracker = new TimeTracker(_eventBus);
        tracker.Restart();
        await Task.Delay(1);
        tracker.UpdateTick();
        Assert.IsTrue(tracker.DeltaTime > TimeSpan.Zero);
        Assert.IsTrue(tracker.CurrentTime > TimeSpan.Zero);
        Assert.IsTrue(tracker.DeltaTime == tracker.CurrentTime);
    }

    [TestMethod]
    public async Task OffsetCorrectsCurrentTime()
    {
        var tracker = new TimeTracker(_eventBus);
        tracker.Restart();
        tracker.SetOffset(TimeSpan.FromSeconds(-60));
        await Task.Delay(1);
        tracker.UpdateTick();
        Assert.IsTrue(tracker.CurrentTime < TimeSpan.Zero);
    }
}
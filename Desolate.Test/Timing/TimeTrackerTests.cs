using Desolate.Eventing;
using Desolate.Services;

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
        Assert.AreEqual(0, tracker.DeltaTime);
        Assert.AreEqual(0, tracker.CurrentTime);
    }

    [TestMethod]
    public async Task AdvancesAfterReset()
    {
        var tracker = new TimeTracker(_eventBus);
        tracker.Restart();
        await Task.Delay(1);
        tracker.UpdateTick();
        Assert.IsTrue(tracker.DeltaTime > 0);
        Assert.IsTrue(tracker.CurrentTime > 0);
        Assert.IsTrue(Math.Abs(tracker.DeltaTime - tracker.CurrentTime) < 0.01);
    }

    [TestMethod]
    public async Task OffsetCorrectsCurrentTime()
    {
        var tracker = new TimeTracker(_eventBus);
        tracker.Restart();
        tracker.SetOffset(-60);
        await Task.Delay(1);
        tracker.UpdateTick();
        Assert.IsTrue(tracker.CurrentTime < 0);
    }
}
using System.ComponentModel.DataAnnotations;
using Desolate.Scheduling;
using Microsoft.Extensions.Logging;
using Moq;

namespace Desolate.Test.Scheduling;

[TestClass]
public sealed class RenderSchedulerTests
{
    private readonly Mock<ILogger<RenderScheduler>> _logger = new(MockBehavior.Strict);

    [TestMethod]
    public async Task CanAsyncAwaitResumeOnThread()
    {
        var scheduler = new RenderScheduler(_logger.Object);
        Thread? thread = null;

        var task = Task.Run(() =>
        {
            thread = Thread.CurrentThread;
            // ReSharper disable once AccessToDisposedClosure
            scheduler.Run();
        });

        var execute1 = false;
        var execute2 = false;

        var source = new TaskCompletionSource<object?>();

        // ReSharper disable once AsyncVoidLambda
        await scheduler.ExecuteAsync(async () =>
        {
            Assert.AreEqual(thread, Thread.CurrentThread);
            await Task.CompletedTask;
            Assert.AreEqual(thread, Thread.CurrentThread);

            execute1 = true;
            await Task.Delay(1);
            Assert.AreEqual(thread, Thread.CurrentThread);

            execute2 = true;
            await Task.Delay(1);
            Assert.AreEqual(thread, Thread.CurrentThread);

            Task.Delay(1).GetAwaiter().GetResult();

            source.SetResult(null);
        });

        await source.Task;
        await scheduler.Shutdown();

        Assert.IsTrue(execute1);
        Assert.IsTrue(execute2);

        await task;
    }

    [TestMethod]
    public async Task AsyncExceptionsWork()
    {
        var scheduler = new RenderScheduler(_logger.Object);

        var task = Task.Run(() =>
        {
            // ReSharper disable once AccessToDisposedClosure
            scheduler.Run();
        });

        var source = new TaskCompletionSource<object?>();

        await scheduler.ExecuteAsync(async () =>
        {
            async Task RaiseAsyncException()
            {
                await Task.Delay(1);
                throw new ValidationException();
            }

            await RaiseAsyncException();
            source.SetResult(null);
        });

        await scheduler.Shutdown();
        await task;
    }

    [TestMethod]
    public async Task DeadlocksWait()
    {
        var scheduler = new RenderScheduler(_logger.Object);

        var task = Task.Run(() =>
        {
            // ReSharper disable once AccessToDisposedClosure
            scheduler.Run();
        });

        async Task AsyncMethod()
        {
            await Task.Delay(1);
        }

        var source = new TaskCompletionSource<bool>();

        Task MainThreadMethod()
        {
            var timeout = AsyncMethod().Wait(100);
            source.SetResult(timeout);
            return Task.CompletedTask;
        }

        await scheduler.ExecuteAsync(MainThreadMethod);

        Assert.IsFalse(await source.Task);

        await scheduler.Shutdown();
        await task;
    }
}
using Desolate.Core.Eventing;
using Desolate.Core.Scheduling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Desolate.Core;

/// <summary>
///     Used to configure and initialize core services
/// </summary>
public static class CoreSetup
{
    /// <summary>
    ///     Registers core services
    /// </summary>
    public static void Register(IHostApplicationBuilder builder)
    {
        // Eventing
        builder.Services.AddSingleton<IEventBus, EventBus>();

        // Scheduling
        builder.Services.AddSingleton<IRenderScheduler, RenderScheduler>();
    }

    /// <summary>
    ///     Initializes core services
    /// </summary>
    public static ValueTask Run(IHost app)
    {
        using var scope = app.Services.CreateScope();

        var scheduler = scope.ServiceProvider.GetRequiredService<IRenderScheduler>();
        scheduler.Run();

        return ValueTask.CompletedTask;
    }
}
using CommunityToolkit.Diagnostics;
using Desolate.Eventing;
using Desolate.Scheduling;
using Desolate.Services;
using Desolate.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Desolate;

/// <summary>
///     Configures and initializes the engine.
/// </summary>
public class EngineRuntime(params string[] args)
{
    /// <summary>
    ///     The core settings object.
    /// </summary>
    protected DesolateOptions? Settings { get; private set; }

    /// <summary>
    ///     Initializes the settings objects.
    /// </summary>
    protected virtual ValueTask SetupSettings(IHostApplicationBuilder builder)
    {
        Settings = builder.Configuration.Get<DesolateOptions>() ??
                   throw new InvalidOperationException("Failed to retrieve settings");

        builder.Services.TryAddSingleton<IValidateOptions<DesolateOptions>, DesolateOptionsValidator>();
        builder.Services.AddOptions<DesolateOptions>()
            .BindConfiguration(string.Empty)
            .ValidateOnStart();

        return ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Configures the logging for the engine.
    /// </summary>
    protected virtual ValueTask ConfigureLogging(IHostApplicationBuilder builder)
    {
        builder.Logging.AddConsole();

        return default;
    }

    /// <summary>
    ///     Configures the core services for the engine.
    /// </summary>
    protected virtual ValueTask ConfigureCore(IHostApplicationBuilder builder)
    {
        builder.Services.TryAddSingleton<IEventBus, EventBus>();

        builder.Services.TryAddScoped<ITimeTracker, TimeTracker>();

        return default;
    }

    /// <summary>
    ///     Configures the client only services.
    /// </summary>
    protected virtual ValueTask ConfigureClient(IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IRenderScheduler, RenderScheduler>();

        return default;
    }

    /// <summary>
    ///     Configures the editor only services.
    /// </summary>
    protected virtual ValueTask ConfigureEditor(IHostApplicationBuilder builder)
    {
        return default;
    }

    /// <summary>
    ///     Configures the server only resources.
    /// </summary>
    protected virtual ValueTask ConfigureServer(IHostApplicationBuilder builder)
    {
        return default;
    }

    /// <summary>
    ///     Configures the web resources.
    /// </summary>
    protected virtual ValueTask ConfigureWeb(IHostApplicationBuilder builder)
    {
        return default;
    }

    /// <summary>
    ///     Allows for setup stuff to be performed prior to the renderer starting.
    /// </summary>
    protected virtual ValueTask PrepareInitialize(IHost app)
    {
        return default;
    }

    /// <summary>
    ///     Starts the engine.
    /// </summary>
    public virtual async ValueTask Run()
    {
        Guard.IsNotNull(Settings);

        var builder = Host.CreateApplicationBuilder(args);

        await ConfigureCore(builder);

        if (Settings.Client.Enabled) await ConfigureClient(builder);

        if (Settings.Editor.Enabled) await ConfigureEditor(builder);

        if (Settings.Server.Enabled) await ConfigureServer(builder);

        if (Settings.Web.Enabled) await ConfigureWeb(builder);

        using var app = builder.Build();

        await PrepareInitialize(app);

        if (Settings.Client.Enabled)
        {
            await app.StartAsync();
            var renderer = app.Services.GetRequiredService<IRenderScheduler>();
            renderer.Run();
            await app.StopAsync();
        }
        else
        {
            await app.RunAsync();
        }
    }
}
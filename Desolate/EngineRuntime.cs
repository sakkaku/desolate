using Desolate.Core.Eventing;
using Desolate.Core.Scheduling;
using Desolate.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Desolate;

public class EngineRuntime(params string[] args)
{
    protected DesolateOptions Settings { get; private set; }
    
    protected virtual ValueTask SetupSettings(IHostApplicationBuilder builder)
    {
        Settings = builder.Configuration.Get<DesolateOptions>() ??
                   throw new InvalidOperationException("Failed to retrieve settings");

        builder.Services.TryAddSingleton<IValidateOptions<DesolateOptions>, DesolateOptionsValidator>();
        builder.Services.AddOptions<DesolateOptions>()
            .BindConfiguration("")
            .ValidateOnStart();
    }

    protected virtual ValueTask ConfigureLogging(IHostApplicationBuilder builder)
    {
        builder.Logging.AddConsole();

        return default;
    }
    
    protected virtual ValueTask ConfigureCore(IHostApplicationBuilder builder)
    {
        builder.Services.TryAddSingleton<IEventBus, EventBus>();
        
        
        return default;
    }

    protected virtual ValueTask ConfigureClient(IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IRenderScheduler, RenderScheduler>();

        return default;
    }
    
    protected virtual ValueTask ConfigureEditor(IHostApplicationBuilder builder)
    {
        return default;
    }

    protected virtual ValueTask ConfigureServer(IHostApplicationBuilder builder)
    {
        return default;
    }
    
    protected virtual ValueTask ConfigureWeb(IHostApplicationBuilder builder)
    {
        return default;
    }

    protected virtual ValueTask PrepareInitialize(IHost app)
    {
        return default;
    }

    public virtual async ValueTask Run()
    {
        var builder = Host.CreateApplicationBuilder(args);
        
        await ConfigureCore(builder);

        if (Settings.Client.Enabled)
        {
            await ConfigureClient(builder);
        }

        if (Settings.Editor.Enabled)
        {
            await ConfigureEditor(builder);
        }

        if (Settings.Server.Enabled)
        {
            await ConfigureServer(builder);
        }

        if (Settings.Web.Enabled)
        {
            await ConfigureWeb(builder);
        }

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
namespace Desolate.Web;

public class DesolateWebHostedService : IHostedService
{
    public Task StartAsync(CancellationToken ct)
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services
            .AddRazorComponents()
            .AddInteractiveServerComponents();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public static void ConfigureDesolateWeb(IHostApplicationBuilder builder)
    {
    }

    public static void InitializeDesolateWeb(IHost host)
    {
    }
}
namespace Desolate.Settings;

public record DesolateOptions
{
    public ClientOptions Client { get; init; } = new();
    
    public ServerOptions Server { get; init; } = new();
    
    public EditorOptions Editor { get; init; } = new();
    
    public WebOptions Web { get; init; } = new();
}
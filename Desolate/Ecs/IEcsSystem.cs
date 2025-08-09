namespace Desolate.Ecs;

/// <summary>
/// Implements the system functionality of ECS.
/// </summary>
public interface IEcsSystem
{
    /// <summary>
    /// The world that this system is registered to.  Will be initialized shortly after adding to world.
    /// </summary>
    public World? World { get; set; }

    /// <summary>
    /// Called when the world is updated.
    /// </summary>
    public ValueTask Update(CancellationToken ct);
}
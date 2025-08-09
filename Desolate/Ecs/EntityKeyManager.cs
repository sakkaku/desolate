namespace Desolate.Ecs;

/// <summary>
/// Handles the assignment and reuse of entity keys
/// </summary>
public sealed class EntityKeyManager
{
    private int _currentId = 1;
    private const int MaxRecycled = 1024;
    private readonly Stack<int> _recycledIds = new (MaxRecycled);
    private readonly Lock _lock = new();

    /// <summary>
    /// Retrieves an id for an entity
    /// </summary>
    public int GetId()
    {
        lock (_lock)
        {
            if (_recycledIds.TryPop(out var id))
            {
                return id;
            }

            return _currentId++;
        }
    }

    /// <summary>
    /// Releases the entity id for recycling
    /// </summary>
    public void ReleaseId(int id)
    {
        lock (_lock)
        {
            if (_recycledIds.Count < MaxRecycled)
            {
                _recycledIds.Push(id);
            }
        }
    }
}
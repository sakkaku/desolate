using System.Diagnostics.CodeAnalysis;

namespace Desolate.Ecs;

/// <summary>
///     Represents a unique instance in the engine
/// </summary>
public sealed class Entity
{
    private readonly Dictionary<Type, IEcsComponent> _components = [];

    /// <summary>
    ///     The identifier for the entity.
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    ///     A human readable name for the entity.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     Returns true if the specified component exists.
    /// </summary>
    public bool HasComponent<T>()
    {
        return HasComponent(typeof(T));
    }

    /// <summary>
    ///     Returns true if the specified component exists.
    /// </summary>
    public bool HasComponent(Type type)
    {
        return _components.ContainsKey(type);
    }

    /// <summary>
    ///     Retrieves the specified component.
    /// </summary>
    public T GetComponent<T>() where T : IEcsComponent
    {
        var type = typeof(T);

        if (_components.TryGetValue(type, out var component)) return (T)component;

        throw new InvalidOperationException($"Component {type.FullName} does not exist on Entity.");
    }

    internal void AttachComponent(Type type, IEcsComponent component)
    {
        _components.Add(type, component);
    }

    internal bool DetachComponent<T>([NotNullWhen(true)] out T? component) where T : IEcsComponent
    {
        if (_components.Remove(typeof(T), out var ecsComponent))
        {
            component = (T)ecsComponent;
            return true;
        }

        component = default;
        return false;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Name} ({Id})";
    }
}
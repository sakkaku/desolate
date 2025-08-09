using System.ComponentModel;
using System.Runtime.CompilerServices;
using Desolate.Ecs;

namespace Desolate.Components;

/// <summary>
///     Represents something that exists in the world.
/// </summary>
public sealed class PositionedObject : IEcsComponent, INotifyPropertyChanged
{
    private Position _position;
    private Rotation _rotation;
    private BoundingBox _boundingBox;

    /// <summary>
    ///     The centerpoint of the object
    /// </summary>
    public Position Position
    {
        get => _position;
        set => SetField(ref _position, value);
    }

    /// <summary>
    ///     The rotation that the object exists on
    /// </summary>
    public Rotation Rotation
    {
        get => _rotation;
        set => SetField(ref _rotation, value);
    }

    /// <summary>
    ///     The bounding box that contains the object.
    /// </summary>
    public BoundingBox BoundingBox
    {
        get => _boundingBox;
        set => SetField(ref _boundingBox, value);
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
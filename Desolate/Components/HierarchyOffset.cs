using System.ComponentModel;
using System.Runtime.CompilerServices;
using Desolate.Ecs;

namespace Desolate.Components;

/// <summary>
///     Provides the ability for an object to be offset from a parent object.
/// </summary>
public sealed class HierarchyOffset : IEcsComponent, INotifyPropertyChanged
{
    private Position _position;
    private Rotation _rotation;

    /// <summary>
    ///     The offset position from the parent.
    /// </summary>
    public Position DeltaPosition
    {
        get => _position;
        set => SetField(ref _position, value);
    }

    /// <summary>
    ///     The offset rotation from the parent.
    /// </summary>
    public Rotation DeltaRotation
    {
        get => _rotation;
        set => SetField(ref _rotation, value);
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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Desolate.Ecs;

namespace Desolate.Components;

/// <summary>
///     Represents a physical object within the engine.
/// </summary>
public sealed class PhysicalObject : IEcsComponent, INotifyPropertyChanged
{
    private AngularVelocity _angularVelocity;
    private PositionalVelocity _positionalVelocity;

    /// <summary>
    ///     The current positional velocity of the object.
    /// </summary>
    public PositionalVelocity PositionalVelocity
    {
        get => _positionalVelocity;
        set => SetField(ref _positionalVelocity, value);
    }

    /// <summary>
    ///     The current angular velocity of the object.
    /// </summary>
    public AngularVelocity AngularVelocity
    {
        get => _angularVelocity;
        set => SetField(ref _angularVelocity, value);
    }

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
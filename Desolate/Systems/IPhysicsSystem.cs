using Silk.NET.Maths;

namespace Desolate.Systems;

public interface IPhysicsSystem
{
    public ValueTask<List<Vector3D<double>>> RayCast(Vector3D<double> start, Vector3D<double> direction,
        Distance maxLength);
}
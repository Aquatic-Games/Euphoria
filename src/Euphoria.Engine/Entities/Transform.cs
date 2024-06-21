using System.Numerics;

namespace Euphoria.Engine.Entities;

public struct Transform
{
    public Vector3 Position;

    public Quaternion Rotation;

    public Vector3 Scale;

    public Vector3 Origin;

    public Transform()
    {
        Position = Vector3.Zero;
        Rotation = Quaternion.Identity;
        Scale = Vector3.One;
        Origin = Vector3.Zero;
    }

    public Transform(Vector3 position, Quaternion rotation, Vector3 scale, Vector3 origin)
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
        Origin = origin;
    }

    public Vector3 Forward => Vector3.Transform(-Vector3.UnitZ, Rotation);

    public Vector3 Backward => Vector3.Transform(Vector3.UnitZ, Rotation);

    public Vector3 Right => Vector3.Transform(Vector3.UnitX, Rotation);

    public Vector3 Left => Vector3.Transform(-Vector3.UnitX, Rotation);

    public Vector3 Up => Vector3.Transform(Vector3.UnitY, Rotation);

    public Vector3 Down => Vector3.Transform(-Vector3.UnitY, Rotation);
}
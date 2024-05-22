using System.Numerics;

namespace u4.Engine.Entities;

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
}
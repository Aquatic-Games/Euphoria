using System.Numerics;

namespace Euphoria.Physics;

public struct BodyDescription
{
    public BodyType BodyType;
    
    public float Mass;

    public Vector3 Position;

    public Quaternion Rotation;

    public Vector3 Scale;

    public BodyDescription(BodyType bodyType, float mass, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        BodyType = bodyType;
        Mass = mass;
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }

    public static BodyDescription Dynamic(float mass, Vector3 position, Quaternion rotation, Vector3 scale)
        => new BodyDescription(BodyType.Dynamic, mass, position, rotation, scale);

    public static BodyDescription Static(Vector3 position, Quaternion rotation, Vector3 scale)
        => new BodyDescription(BodyType.Static, 0, position, rotation, scale);
}
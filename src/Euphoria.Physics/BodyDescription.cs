using System.Numerics;

namespace Euphoria.Physics;

public struct BodyDescription
{
    public BodyType BodyType;
    
    public float Mass;

    public Vector3 Position;

    public Quaternion Rotation;

    public BodyDescription(BodyType bodyType, float mass, Vector3 position, Quaternion rotation)
    {
        BodyType = bodyType;
        Mass = mass;
        Position = position;
        Rotation = rotation;
    }

    public static BodyDescription Dynamic(float mass, Vector3 position, Quaternion rotation)
        => new BodyDescription(BodyType.Dynamic, mass, position, rotation);

    public static BodyDescription Static(Vector3 position, Quaternion rotation)
        => new BodyDescription(BodyType.Static, 0, position, rotation);
}
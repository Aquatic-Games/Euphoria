using System.Numerics;

namespace Euphoria.Physics;

public struct BodyDescription
{
    public BodyType BodyType;
    
    public float Mass;

    public Vector3 Position;

    public Quaternion Rotation;

    public Vector3 Scale;

    public CollisionType CollisionType;

    public BodyDescription(BodyType bodyType, float mass, Vector3 position, Quaternion rotation, Vector3 scale, CollisionType collisionType = CollisionType.Solid)
    {
        BodyType = bodyType;
        Mass = mass;
        Position = position;
        Rotation = rotation;
        Scale = scale;
        CollisionType = collisionType;
    }

    public static BodyDescription Dynamic(float mass, Vector3 position, Quaternion rotation, Vector3 scale, CollisionType collisionType = CollisionType.Solid)
        => new BodyDescription(BodyType.Dynamic, mass, position, rotation, scale, collisionType);

    public static BodyDescription Static(Vector3 position, Quaternion rotation, Vector3 scale, CollisionType collisionType = CollisionType.Solid)
        => new BodyDescription(BodyType.Static, 0, position, rotation, scale, collisionType);
}
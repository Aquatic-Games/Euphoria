using System.Numerics;

namespace Euphoria.Physics;

public struct BodyDescription
{
    public BodyType BodyType;

    public Vector3 Position;

    public Quaternion Rotation;

    public BodyDescription(BodyType bodyType, Vector3 position, Quaternion rotation)
    {
        BodyType = bodyType;
        Position = position;
        Rotation = rotation;
    }

    public static BodyDescription Dynamic(Vector3 position, Quaternion rotation)
        => new BodyDescription(BodyType.Dynamic, position, rotation);

    public static BodyDescription Static(Vector3 position, Quaternion rotation)
        => new BodyDescription(BodyType.Static, position, rotation);
}
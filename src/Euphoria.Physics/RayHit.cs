using System.Numerics;

namespace Euphoria.Physics;

public readonly struct RayHit
{
    public readonly Body Body;
    
    public readonly Vector3 Position;

    public readonly Vector3 Normal;

    public RayHit(Body body, Vector3 position, Vector3 normal)
    {
        Body = body;
        Position = position;
        Normal = normal;
    }
}
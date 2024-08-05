using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Trees;

namespace Euphoria.Physics.Internal;

internal struct RayHitHandler : IRayHitHandler
{
    public bool HasHit;

    public float Distance;

    public Vector3 Normal;

    public int ChildIndex;

    public CollidableReference Collidable;

    public RayHitHandler()
    {
        HasHit = false;
        Distance = float.MaxValue;
    }
    
    public bool AllowTest(CollidableReference collidable)
    {
        return true;
    }

    public bool AllowTest(CollidableReference collidable, int childIndex)
    {
        return true;
    }

    public void OnRayHit(in RayData ray, ref float maximumT, float t, Vector3 normal, CollidableReference collidable,
        int childIndex)
    {
        HasHit = true;

        if (t > Distance)
            return;
        
        Distance = t;
        Normal = normal;
        ChildIndex = childIndex;
        Collidable = collidable;
    }
}
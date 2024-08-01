using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;

namespace Euphoria.Physics;

public struct Body
{
    public readonly CollidableReference Reference;

    public ref Vector3 Position
    {
        get
        {
            Simulation simulation = PhysicsWorld.Simulation;
            return ref simulation.Bodies[Reference.BodyHandle].Pose.Position;
        }
    }

    public ref Quaternion Rotation
    {
        get
        {
            Simulation simulation = PhysicsWorld.Simulation;
            return ref simulation.Bodies[Reference.BodyHandle].Pose.Orientation;
        }
    }

    public Body(CollidableReference reference)
    {
        Reference = reference;
    }
}
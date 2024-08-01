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
            switch (Reference.Mobility)
            {
                case CollidableMobility.Dynamic:
                case CollidableMobility.Kinematic:
                    return ref simulation.Bodies[Reference.BodyHandle].Pose.Position;
                
                case CollidableMobility.Static:
                    return ref simulation.Statics[Reference.StaticHandle].Pose.Position;
                    
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public ref Quaternion Rotation
    {
        get
        {
            Simulation simulation = PhysicsWorld.Simulation;
            switch (Reference.Mobility)
            {
                case CollidableMobility.Dynamic:
                case CollidableMobility.Kinematic:
                    return ref simulation.Bodies[Reference.BodyHandle].Pose.Orientation;
                
                case CollidableMobility.Static:
                    return ref simulation.Statics[Reference.StaticHandle].Pose.Orientation;
                    
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public Body(CollidableReference reference)
    {
        Reference = reference;
    }
}
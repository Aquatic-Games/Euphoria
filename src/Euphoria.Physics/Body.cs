using System;
using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;

namespace Euphoria.Physics;

public struct Body
{
    private readonly CollidableReference _collidable;

    public ulong Id => _collidable.Packed;

    public ref Vector3 Position
    {
        get
        {
            Simulation simulation = PhysicsWorld.Simulation;
            switch (_collidable.Mobility)
            {
                case CollidableMobility.Dynamic:
                case CollidableMobility.Kinematic:
                    return ref simulation.Bodies[_collidable.BodyHandle].Pose.Position;
                
                case CollidableMobility.Static:
                    return ref simulation.Statics[_collidable.StaticHandle].Pose.Position;
                    
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
            switch (_collidable.Mobility)
            {
                case CollidableMobility.Dynamic:
                case CollidableMobility.Kinematic:
                    return ref simulation.Bodies[_collidable.BodyHandle].Pose.Orientation;
                
                case CollidableMobility.Static:
                    return ref simulation.Statics[_collidable.StaticHandle].Pose.Orientation;
                    
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public Body(CollidableReference collidable)
    {
        _collidable = collidable;
    }
}
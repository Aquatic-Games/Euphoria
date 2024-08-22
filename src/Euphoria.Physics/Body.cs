using System;
using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;

namespace Euphoria.Physics;

public struct Body
{
    private readonly CollidableReference _collidable;

    public ulong Id => _collidable.Packed;

    public Vector3 Position
    {
        get
        {
            Simulation simulation = PhysicsWorld.Simulation;
            return GetPose(simulation, _collidable).Position;
        }
        set
        {
            Simulation simulation = PhysicsWorld.Simulation;
            GetPose(simulation, _collidable).Position = value;
        }
    }

    public Quaternion Rotation
    {
        get
        {
            Simulation simulation = PhysicsWorld.Simulation;
            return GetPose(simulation, _collidable).Orientation;
        }
        set
        {
            Simulation simulation = PhysicsWorld.Simulation;
            GetPose(simulation, _collidable).Orientation = value;
        }
    }

    public Vector3 LinearVelocity
    {
        get
        {
            Simulation simulation = PhysicsWorld.Simulation;
            switch (_collidable.Mobility)
            {
                case CollidableMobility.Dynamic:
                case CollidableMobility.Kinematic:
                    return simulation.Bodies[_collidable.BodyHandle].Velocity.Linear;
                
                case CollidableMobility.Static:
                    return Vector3.Zero;
                    
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        set
        {
            Simulation simulation = PhysicsWorld.Simulation;
            
            switch (_collidable.Mobility)
            {
                case CollidableMobility.Dynamic:
                case CollidableMobility.Kinematic:
                    simulation.Bodies[_collidable.BodyHandle].Velocity.Linear = value;
                    break;
                
                case CollidableMobility.Static:
                    break;
                    
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    public Vector3 AngularVelocity
    {
        get
        {
            Simulation simulation = PhysicsWorld.Simulation;
            switch (_collidable.Mobility)
            {
                case CollidableMobility.Dynamic:
                case CollidableMobility.Kinematic:
                    return simulation.Bodies[_collidable.BodyHandle].Velocity.Angular;
                
                case CollidableMobility.Static:
                    return Vector3.Zero;
                    
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        set
        {
            Simulation simulation = PhysicsWorld.Simulation;
            
            switch (_collidable.Mobility)
            {
                case CollidableMobility.Dynamic:
                case CollidableMobility.Kinematic:
                    simulation.Bodies[_collidable.BodyHandle].Velocity.Linear = value;
                    break;
                
                case CollidableMobility.Static:
                    break;
                    
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public Body(CollidableReference collidable)
    {
        _collidable = collidable;
    }

    public void UpdateBounds()
    {
        Simulation simulation = PhysicsWorld.Simulation;
        BodyReference reference = simulation.Bodies[_collidable.BodyHandle];
        reference.Awake = true;
        reference.UpdateBounds();
    }

    private static ref RigidPose GetPose(Simulation simulation, CollidableReference collidable)
    {
        switch (collidable.Mobility)
        {
            case CollidableMobility.Dynamic:
            case CollidableMobility.Kinematic:
                return ref simulation.Bodies[collidable.BodyHandle].Pose;
                
            case CollidableMobility.Static:
                return ref simulation.Statics[collidable.StaticHandle].Pose;
                    
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
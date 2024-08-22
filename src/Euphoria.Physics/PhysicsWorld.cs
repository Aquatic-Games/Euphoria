using System;
using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities;
using BepuUtilities.Memory;
using Euphoria.Physics.Internal;
using Euphoria.Physics.Internal.Callbacks;
using IShape = Euphoria.Physics.Shapes.IShape;

namespace Euphoria.Physics;

public static class PhysicsWorld
{
    public static event OnBodyContact BodyContact = delegate { };

    private static NarrowPhaseCallbacks _narrowPhaseCallbacks;
    private static PoseIntegratorCallbacks _poseIntegratorCallbacks;
    
    private static BufferPool _bufferPool;
    
    internal static ThreadDispatcher ThreadDispatcher;
    internal static Simulation Simulation;

    public static Vector3 Gravity
    {
        get => _poseIntegratorCallbacks.Gravity;
        set => _poseIntegratorCallbacks.Gravity = value;
    }

    public static void Initialize()
    {
        _narrowPhaseCallbacks = new NarrowPhaseCallbacks(ContactGenerationCallback);
        _poseIntegratorCallbacks = new PoseIntegratorCallbacks(new Vector3(0, -9.81f, 0));
        
        _bufferPool = new BufferPool();
        ThreadDispatcher = new ThreadDispatcher(Environment.ProcessorCount);

        Simulation = Simulation.Create(_bufferPool, _narrowPhaseCallbacks, _poseIntegratorCallbacks,
            new SolveDescription(8, 1));
    }

    public static Body CreateBody(in BodyDescription description, IShape shape)
    {
        BodyInertia inertia = shape.CalculateInertia(description.Mass);
        TypedIndex index = shape.AddToSimulation(Simulation, description);
        RigidPose pose = new RigidPose(description.Position, description.Rotation);

        CollidableReference reference;
        
        switch (description.BodyType)
        {
            case BodyType.Dynamic:
            {
                BepuPhysics.BodyDescription desc =
                    BepuPhysics.BodyDescription.CreateDynamic(pose, inertia, index, new BodyActivityDescription(0.01f));

                BodyHandle handle = Simulation.Bodies.Add(desc);
                reference = new CollidableReference(CollidableMobility.Dynamic, handle);
                break;
            }
            
            case BodyType.Kinematic:
                throw new NotImplementedException();

            case BodyType.Static:
            {
                StaticDescription desc = new StaticDescription(pose, index);

                StaticHandle handle = Simulation.Statics.Add(desc);
                reference = new CollidableReference(handle);
                break;
            }
            
            default:
                throw new ArgumentOutOfRangeException();
        }

        _narrowPhaseCallbacks.CollisionTypes.Allocate(reference) = description.CollisionType;
        return new Body(reference);
    }

    public static bool Raycast(Vector3 position, Vector3 direction, float maxDistance, out RayHit hit)
    {
        RayHitHandler hitHandler = new RayHitHandler();
        Simulation.RayCast(position, direction, maxDistance, ref hitHandler);

        if (hitHandler.HasHit)
        {
            Vector3 hitPos = position + (direction * hitHandler.Distance);

            hit = new RayHit(new Body(hitHandler.Collidable), hitPos, hitHandler.Normal, hitHandler.ChildIndex);
            return true;
        }

        hit = new RayHit();
        return false;
    }

    public static void Tick(float dt)
    {
        Simulation.Timestep(dt, ThreadDispatcher);
    }

    private static void ContactGenerationCallback(CollidableReference a, CollidableReference b)
    {
        BodyContact.Invoke(new Body(a), new Body(b));
    }

    public delegate void OnBodyContact(Body a, Body b);
}
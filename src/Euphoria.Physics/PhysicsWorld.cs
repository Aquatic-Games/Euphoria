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
    private static NarrowPhaseCallbacks _narrowPhaseCallbacks;
    private static PoseIntegratorCallbacks _poseIntegratorCallbacks;
    
    private static BufferPool _bufferPool;
    private static ThreadDispatcher _threadDispatcher;
    
    internal static Simulation Simulation;

    public static Vector3 Gravity
    {
        get => _poseIntegratorCallbacks.Gravity;
        set => _poseIntegratorCallbacks.Gravity = value;
    }

    public static void Initialize()
    {
        _narrowPhaseCallbacks = new NarrowPhaseCallbacks();
        _poseIntegratorCallbacks = new PoseIntegratorCallbacks(new Vector3(0, -9.81f, 0));
        
        _bufferPool = new BufferPool();
        _threadDispatcher = new ThreadDispatcher(Environment.ProcessorCount);

        Simulation = Simulation.Create(_bufferPool, _narrowPhaseCallbacks, _poseIntegratorCallbacks,
            new SolveDescription(8, 1));
    }

    public static Body CreateBody(in BodyDescription description, IShape shape)
    {
        BodyInertia inertia = shape.CalculateInertia(description.Mass);
        TypedIndex index = shape.AddToSimulation(Simulation, description);
        RigidPose pose = new RigidPose(description.Position, description.Rotation);
        
        switch (description.BodyType)
        {
            case BodyType.Dynamic:
            {
                BepuPhysics.BodyDescription desc =
                    BepuPhysics.BodyDescription.CreateDynamic(pose, inertia, index, new BodyActivityDescription(0.01f));

                BodyHandle handle = Simulation.Bodies.Add(desc);
                return new Body(new CollidableReference(CollidableMobility.Dynamic, handle));
            }
            
            case BodyType.Kinematic:
                throw new NotImplementedException();

            case BodyType.Static:
            {
                StaticDescription desc = new StaticDescription(pose, index);

                StaticHandle handle = Simulation.Statics.Add(desc);
                return new Body(new CollidableReference(handle));
            }
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static bool Raycast(Vector3 position, Vector3 direction, float maxDistance, out RayHit hit)
    {
        RayHitHandler hitHandler = new RayHitHandler();
        Simulation.RayCast(position, direction, maxDistance, ref hitHandler);

        if (hitHandler.HasHit)
        {
            Vector3 hitPos = position + (direction * hitHandler.Distance);
            
            hit = new RayHit(new Body(hitHandler.Collidable), hitPos, hitHandler.Normal);
            return true;
        }

        hit = new RayHit();
        return false;
    }

    public static void Tick(float dt)
    {
        Simulation.Timestep(dt, _threadDispatcher);
    }
}
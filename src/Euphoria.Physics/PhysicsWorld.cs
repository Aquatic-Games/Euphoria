using BepuPhysics;
using BepuUtilities;
using BepuUtilities.Memory;
using Euphoria.Physics.Callbacks;

namespace Euphoria.Physics;

public static class PhysicsWorld
{
    private static NarrowPhaseCallbacks _narrowPhaseCallbacks;
    private static PoseIntegratorCallbacks _poseIntegratorCallbacks;
    
    private static BufferPool _bufferPool;
    private static ThreadDispatcher _threadDispatcher;
    
    internal static Simulation Simulation;

    public static void Initialize()
    {
        _narrowPhaseCallbacks = new NarrowPhaseCallbacks();
        _poseIntegratorCallbacks = new PoseIntegratorCallbacks();
        
        _bufferPool = new BufferPool();
        _threadDispatcher = new ThreadDispatcher(Environment.ProcessorCount);

        Simulation = Simulation.Create(_bufferPool, _narrowPhaseCallbacks, _poseIntegratorCallbacks,
            new SolveDescription(8, 1));
    }

    public static Body CreateBody(BodyDescription description)
    {
        switch (description.BodyType)
        {
            case BodyType.Dynamic:
            {
                BepuPhysics.BodyDescription desc = BepuPhysics.BodyDescription.CreateDynamic()
                
                break;
            }
            
            case BodyType.Kinematic:
                break;
            case BodyType.Static:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static void Tick(float dt)
    {
        Simulation.Timestep(dt, _threadDispatcher);
    }
}
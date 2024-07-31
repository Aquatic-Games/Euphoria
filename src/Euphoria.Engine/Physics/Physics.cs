using System;
using BepuPhysics;
using BepuUtilities;
using BepuUtilities.Memory;
using Euphoria.Engine.Physics.Callbacks;

namespace Euphoria.Engine.Physics;

public static class Physics
{
    private static NarrowPhaseCallbacks _narrowPhaseCallbacks;
    private static PoseIntegratorCallbacks _poseIntegratorCallbacks;
    
    private static BufferPool _bufferPool;
    private static ThreadDispatcher _threadDispatcher;
    
    public static Simulation Simulation;

    internal static void Initialize()
    {
        _narrowPhaseCallbacks = new NarrowPhaseCallbacks();
        _poseIntegratorCallbacks = new PoseIntegratorCallbacks();
        
        _bufferPool = new BufferPool();
        _threadDispatcher = new ThreadDispatcher(Environment.ProcessorCount);

        Simulation = Simulation.Create(_bufferPool, _narrowPhaseCallbacks, _poseIntegratorCallbacks,
            new SolveDescription(8, 1));
    }

    internal static void Tick(float dt)
    {
        Simulation.Timestep(dt, _threadDispatcher);
    }
}
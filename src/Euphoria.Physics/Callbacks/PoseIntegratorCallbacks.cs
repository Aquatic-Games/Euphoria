using System.Numerics;
using BepuPhysics;
using BepuUtilities;

namespace Euphoria.Physics.Callbacks;

internal struct PoseIntegratorCallbacks : IPoseIntegratorCallbacks
{
    private Vector3Wide _gravityWide;
    
    public Vector3 Gravity;

    public PoseIntegratorCallbacks(Vector3 initialGravity)
    {
        Gravity = initialGravity;
    }
    
    public void Initialize(Simulation simulation)
    {
       
    }

    public void PrepareForIntegration(float dt)
    {
        _gravityWide = Vector3Wide.Broadcast(Gravity * dt);
    }

    public void IntegrateVelocity(Vector<int> bodyIndices, Vector3Wide position, QuaternionWide orientation,
        BodyInertiaWide localInertia, Vector<int> integrationMask, int workerIndex, Vector<float> dt, ref BodyVelocityWide velocity)
    {
        velocity.Linear += _gravityWide;
    }

    public AngularIntegrationMode AngularIntegrationMode => AngularIntegrationMode.Nonconserving;

    public bool AllowSubstepsForUnconstrainedBodies => false;

    public bool IntegrateVelocityForKinematics => false;
}
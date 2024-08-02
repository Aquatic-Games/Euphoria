using BepuPhysics;
using BepuPhysics.Collidables;

namespace Euphoria.Physics.Shapes;

public interface IShape
{
    internal BodyInertia CalculateInertia(float mass);
    
    internal TypedIndex AddToSimulation(Simulation simulation, in BodyDescription description);
}
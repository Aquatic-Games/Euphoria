using BepuPhysics;
using BepuPhysics.Collidables;

namespace Euphoria.Physics.Shapes;

public struct Box : IShape
{
    public BepuPhysics.Collidables.Box BepuBox;
    
    public Box(float width, float height, float depth)
    {
        BepuBox = new BepuPhysics.Collidables.Box(width, height, depth);
    }

    public BodyInertia CalculateInertia(float mass)
    {
        return BepuBox.ComputeInertia(mass);
    }

    public TypedIndex AddToSimulation(Simulation simulation)
    {
        return simulation.Shapes.Add(BepuBox);
    }
}
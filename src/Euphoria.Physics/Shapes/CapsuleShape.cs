using BepuPhysics;
using BepuPhysics.Collidables;

namespace Euphoria.Physics.Shapes;

public class CapsuleShape : IShape
{
    private Capsule _capsule;

    public float Width
    {
        get => _capsule.Radius * 2;
        set => _capsule.Radius = value * 0.5f;
    }

    public float Height
    {
        get => _capsule.Length;
        set => _capsule.Length = value;
    }

    public CapsuleShape(float width, float height)
    {
        _capsule = new Capsule(width * 0.5f, height);
    }
    
    public BodyInertia CalculateInertia(float mass)
    {
        return _capsule.ComputeInertia(mass);
    }

    public TypedIndex AddToSimulation(Simulation simulation, in BodyDescription description)
    {
        return simulation.Shapes.Add(_capsule);
    }
}
﻿using BepuPhysics;
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

    public TypedIndex AddToSimulation(Simulation simulation, in BodyDescription description)
    {
        BepuPhysics.Collidables.Box box = BepuBox;
        box.HalfWidth *= description.Scale.X;
        box.HalfHeight *= description.Scale.Y;
        box.HalfLength *= description.Scale.Z;
        
        return simulation.Shapes.Add(box);
    }
}
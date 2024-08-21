using System;
using System.Collections.Generic;
using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Memory;

namespace Euphoria.Physics.Shapes;

public class CompoundShape : IShape
{
    private CompoundBuilder _builder;
    private List<float> _masses;
    private bool _isBuilt;
    private BigCompound _compound;

    public CompoundShape(int initialCapacity = 16)
    {
        _builder = new CompoundBuilder(PhysicsWorld.Simulation.BufferPool, PhysicsWorld.Simulation.Shapes,
            initialCapacity);

        _masses = new List<float>();
    }

    public void Add(in Child child)
    {
        _masses.Add(child.Mass);
        Matrix4x4.Decompose(child.Transform, out Vector3 scale, out Quaternion rotation, out Vector3 position);
        RigidPose pose = new RigidPose(position, rotation);
        TypedIndex shape = child.Shape.AddToSimulation(PhysicsWorld.Simulation,
            new BodyDescription(BodyType.Dynamic, child.Mass, position, rotation, scale));
        
        if (_isBuilt)
        {
            _compound.Add(new CompoundChild(new RigidPose(position, rotation), shape),
                PhysicsWorld.Simulation.BufferPool, PhysicsWorld.Simulation.Shapes);
        }
        else
        {
            _builder.Add(shape, pose, child.Shape.CalculateInertia(child.Mass));
        }
    }

    public BodyInertia CalculateInertia(float mass)
    {
        if (_isBuilt)
            throw new NotImplementedException();
        else
        {
            CompoundChild[] children = new CompoundChild[_builder.Children.Count];
            for (int i = 0; i < _builder.Children.Count; i++)
                children[i] = new CompoundChild(_builder.Children[i].LocalPose, _builder.Children[i].ShapeIndex);

            return CompoundBuilder.ComputeInertia(children, _masses.ToArray(), PhysicsWorld.Simulation.Shapes);
        }
    }

    public TypedIndex AddToSimulation(Simulation simulation, in BodyDescription description)
    {
        if (_isBuilt)
            throw new NotSupportedException();

        _isBuilt = true;
        
        _builder.BuildDynamicCompound(out Buffer<CompoundChild> children, out BodyInertia inertia);
        _compound = new BigCompound(children, PhysicsWorld.Simulation.Shapes, PhysicsWorld.Simulation.BufferPool,
            PhysicsWorld.ThreadDispatcher);
        
        _builder.Dispose();

        return simulation.Shapes.Add(_compound);
    }

    public struct Child
    {
        public Matrix4x4 Transform;
        public IShape Shape;
        public float Mass;

        public Child(Matrix4x4 transform, IShape shape, float mass)
        {
            Transform = transform;
            Shape = shape;
            Mass = mass;
        }
    }
}
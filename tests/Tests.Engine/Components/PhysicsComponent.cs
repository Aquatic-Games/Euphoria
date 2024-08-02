using System;
using System.Numerics;
using Euphoria.Engine;
using Euphoria.Engine.Entities;
using Euphoria.Engine.Entities.Components;
using Euphoria.Physics;
using Euphoria.Physics.Shapes;

namespace Tests.Engine.Components;

public class PhysicsComponent : Component
{
    private IShape _shape;
    private float _mass;
    private Body _body;

    private Transform _previousTransform;
    private Transform _currentTransform;

    public PhysicsComponent(IShape shape, float mass)
    {
        _shape = shape;
        _mass = mass;
    }

    public override void Initialize()
    {
        if (_mass == 0)
            _body = PhysicsWorld.CreateBody(BodyDescription.Static(Transform.Position, Transform.Rotation), _shape);
        else
            _body = PhysicsWorld.CreateBody(BodyDescription.Dynamic(_mass, Transform.Position, Transform.Rotation), _shape);

        _previousTransform = Transform;
        _currentTransform = Transform;
    }

    public override void Tick(float dt)
    {
        _previousTransform = _currentTransform;
            
        _currentTransform.Position = _body.Position;
        _currentTransform.Rotation = _body.Rotation;
    }

    public override void Update(float dt)
    {
        Transform = Transform.Lerp(_previousTransform, _currentTransform, (float) App.TickInterpolation);
    }
}
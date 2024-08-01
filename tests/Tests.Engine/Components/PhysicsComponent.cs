using System;
using Euphoria.Engine.Entities.Components;
using Euphoria.Physics;
using Euphoria.Physics.Shapes;

namespace Tests.Engine.Components;

public class PhysicsComponent : Component
{
    private IShape _shape;
    private float _mass;
    private Body _body;

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
    }

    public override void Tick(float dt)
    {
        Transform.Position = _body.Position;
        Transform.Rotation = _body.Rotation;
    }
}
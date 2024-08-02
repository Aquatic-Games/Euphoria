using System;
using Euphoria.Physics;
using Euphoria.Physics.Shapes;

namespace Euphoria.Engine.Entities.Components;

public class Rigidbody : Component
{
    private readonly float _mass;
    private readonly bool _interpolate;

    private Transform _prevTransform;
    private Transform _newTransform;

    private Body _body;
    
    public readonly IShape Shape;
    
    public Rigidbody(IShape shape, float mass, bool interpolate = true)
    {
        Shape = shape;
        _mass = mass;
        _interpolate = interpolate;
    }

    public override void Initialize()
    {
        _prevTransform = Transform;
        _newTransform = Transform;
        
        BodyDescription description;
        
        if (_mass == 0)
            description = BodyDescription.Static(Transform.Position, Transform.Rotation, Transform.Scale);
        else
            description = BodyDescription.Dynamic(_mass, Transform.Position, Transform.Rotation, Transform.Scale);

        _body = PhysicsWorld.CreateBody(description, Shape);
    }

    public override void Tick(float dt)
    {
        if (Transform.Scale != _prevTransform.Scale)
        {
            throw new NotImplementedException();
        }
        
        if (_interpolate)
        {
            _prevTransform = _newTransform;
            _newTransform.Position = _body.Position;
            _newTransform.Rotation = _body.Rotation;
        }
        else
        {
            _prevTransform = Transform;
            Transform.Position = _body.Position;
            Transform.Rotation = _body.Rotation;
        }
    }

    public override void Update(float dt)
    {
        if (!_interpolate)
            return;
        
        Transform = Transform.Lerp(_prevTransform, _newTransform, (float) App.TickInterpolation);
    }
}
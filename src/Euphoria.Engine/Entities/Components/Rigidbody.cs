using System;
using System.Numerics;
using Euphoria.Engine.Scenes;
using Euphoria.Physics;
using Euphoria.Physics.Shapes;

namespace Euphoria.Engine.Entities.Components;

public class Rigidbody : Component
{
    private readonly float _mass;
    private CollisionType _collisionType;
    private readonly bool _interpolate;

    private Transform _prevTransform;
    private Transform _newTransform;

    private Body _body;
    
    public readonly IShape Shape;

    public ref Vector3 LinearVelocity => ref _body.LinearVelocity;

    public ref Vector3 AngularVelocity => ref _body.AngularVelocity;
    
    public Rigidbody(IShape shape, float mass, bool interpolate = true, CollisionType collisionType = CollisionType.Solid)
    {
        Shape = shape;
        _mass = mass;
        _interpolate = interpolate;
        _collisionType = collisionType;
    }
    
    public void Teleport(Vector3 position)
    {
        _body.Position = position;
    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        _body.Position = position;
        _body.Rotation = rotation;
    }

    public void ResetVelocity()
    {
        LinearVelocity = Vector3.Zero;
        AngularVelocity = Vector3.Zero;
    }

    public override void Initialize()
    {
        _prevTransform = Transform;
        _newTransform = Transform;
        
        BodyDescription description;
        
        if (_mass == 0)
            description = BodyDescription.Static(Transform.Position, Transform.Rotation, Transform.Scale, _collisionType);
        else
            description = BodyDescription.Dynamic(_mass, Transform.Position, Transform.Rotation, Transform.Scale, _collisionType);

        _body = PhysicsWorld.CreateBody(description, Shape);
        // TODO: These types of functions should be directly in the component so we don't need to get the active scene each time.
        SceneManager.ActiveScene.BodyIdToEntity.Add(_body.Id, Entity);
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
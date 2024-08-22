using System;
using System.Numerics;
using Euphoria.Engine.Scenes;
using Euphoria.Physics;
using Euphoria.Physics.Shapes;

namespace Euphoria.Engine.Entities.Components;

public class Rigidbody : Component
{
    public event OnCollisionDetected CollisionDetected;
    
    private readonly float _mass;
    private readonly CollisionType _collisionType;
    private readonly bool _interpolate;

    private bool _isCollisionDetectedCallbackUsed;

    private Transform _prevTransform;
    private Transform _newTransform;

    private Body _body;
    
    public readonly IShape Shape;

    public Vector3 LinearVelocity
    {
        get => _body.LinearVelocity;
        set => _body.LinearVelocity = value;
    }

    public Vector3 AngularVelocity
    {
        get => _body.AngularVelocity;
        set => _body.AngularVelocity = value;
    }
    
    public Rigidbody(IShape shape, float mass, bool interpolate = true, CollisionType collisionType = CollisionType.Solid)
    {
        Shape = shape;
        _mass = mass;
        _interpolate = interpolate;
        _collisionType = collisionType;

        // No point interpolating statics.
        if (_mass == 0)
            _interpolate = false;
    }
    
    public void Teleport(Vector3 position)
    {
        _body.Position = position;
        _body.UpdateBounds();
    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        _body.Position = position;
        _body.Rotation = rotation;
        _body.UpdateBounds();
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
        // Optimization: Only link the body contact callback if the collision detected callback is *actually* used.
        if (CollisionDetected != null && !_isCollisionDetectedCallbackUsed)
        {
            _isCollisionDetectedCallbackUsed = true;
            PhysicsWorld.BodyContact += OnContact;
        }
        
        if (!_interpolate)
            return;
        
        Transform = Transform.Lerp(_prevTransform, _newTransform, (float) App.TickInterpolation);
    }

    private void OnContact(Body a, Body b)
    {
        if (a.Id == _body.Id)
        {
            CollisionDetected?.Invoke(SceneManager.ActiveScene.BodyIdToEntity[b.Id]);
            return;
        }
        
        if (b.Id == _body.Id)
            CollisionDetected?.Invoke(SceneManager.ActiveScene.BodyIdToEntity[a.Id]);
    }

    public override void Dispose()
    {
        PhysicsWorld.BodyContact -= OnContact;
    }

    public delegate void OnCollisionDetected(Entity entity);
}
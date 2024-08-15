using System;
using System.Numerics;
using Euphoria.Engine;
using Euphoria.Engine.Entities.Components;
using Euphoria.Engine.InputSystem;
using Euphoria.Engine.InputSystem.Actions;

namespace Tests.Engine.Components;

public class CameraMove : Component
{
    private Vector3 _rotation;

    private InputAction _move;
    private InputAction _jump;
    private InputAction _crouch;
    private InputAction _look;

    public override void Initialize()
    {
        ActionSet mainScene = Input.GetActionSet("Main");

        _move = mainScene.GetAction("Move");
        _jump = mainScene.GetAction("Jump");
        _crouch = mainScene.GetAction("Crouch");
        _look = mainScene.GetAction("Look");
    }

    public override void Update(float dt)
    {
        float speed = 5 * dt;

        Vector2 move = _move.GetVector2();

        Transform.Position += Transform.Forward * move.Y * speed;
        Transform.Position += Transform.Right * move.X * speed;

        if (_jump.IsDown)
            Transform.Position += Transform.Up * speed;
        if (_crouch.IsDown)
            Transform.Position += Transform.Down * speed;

        _rotation += _look.GetVector3();
        _rotation.Y = float.Clamp(_rotation.Y, -MathF.PI / 2, MathF.PI / 2);

        Transform.Rotation = Quaternion.CreateFromYawPitchRoll(_rotation.X, _rotation.Y, _rotation.Z);
    }
}
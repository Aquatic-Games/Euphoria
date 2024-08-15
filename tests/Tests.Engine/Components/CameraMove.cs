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

    private Action2D _move;
    private ButtonAction _jump;
    private ButtonAction _crouch;
    private Action2D _look;

    public override void Initialize()
    {
        ActionSet mainScene = Input.GetActionSet("Main");

        _move = mainScene.GetAction<Action2D>("Move");
        _jump = mainScene.GetAction<ButtonAction>("Jump");
        _crouch = mainScene.GetAction<ButtonAction>("Crouch");
        _look = mainScene.GetAction<Action2D>("Look");
    }

    public override void Update(float dt)
    {
        float speed = 5 * dt;

        Transform.Position += Transform.Forward * _move.Value.Y * speed;
        Transform.Position += Transform.Right * _move.Value.X * speed;

        if (_jump.IsDown)
            Transform.Position += Transform.Up * speed;
        if (_crouch.IsDown)
            Transform.Position += Transform.Down * speed;

        _rotation += new Vector3(_look.Value, 0);
        _rotation.Y = float.Clamp(_rotation.Y, -MathF.PI / 2, MathF.PI / 2);

        Transform.Rotation = Quaternion.CreateFromYawPitchRoll(_rotation.X, _rotation.Y, _rotation.Z);
    }
}
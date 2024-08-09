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

    private DirectionalAction _move;
    private ButtonAction _jump;
    private ButtonAction _crouch;
    private DirectionalAction _look;

    public override void Initialize()
    {
        InputScene mainScene = Input.GetInputScene("Main");

        _move = mainScene.GetAction<DirectionalAction>("Move");
        _jump = mainScene.GetAction<ButtonAction>("Jump");
        _crouch = mainScene.GetAction<ButtonAction>("Crouch");
        _look = mainScene.GetAction<DirectionalAction>("Look");
    }

    public override void Update(float dt)
    {
        if (Input.UIWantsFocus)
            return;
        
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
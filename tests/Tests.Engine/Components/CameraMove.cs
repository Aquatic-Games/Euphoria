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

    private DualAxisAction _move;
    private DualAxisAction _look;

    public override void Initialize()
    {
        InputScene mainScene = Input.GetInputScene("Main");

        _move = mainScene.GetAction<DualAxisAction>("Move");
        _look = mainScene.GetAction<DualAxisAction>("Look");
    }

    public override void Update(float dt)
    {
        if (Input.UIWantsFocus)
            return;
        
        float speed = 5 * dt;

        Transform.Position += Transform.Forward * _move.Value.Y * speed;
        Transform.Position += Transform.Right * _move.Value.X * speed;

        if (Input.IsKeyDown(Key.Space))
            Transform.Position += Transform.Up * speed;
        if (Input.IsKeyDown(Key.LeftControl))
            Transform.Position += Transform.Down * speed;

        _rotation += new Vector3(_look.Value, 0);
        _rotation.Y = float.Clamp(_rotation.Y, -MathF.PI / 2, MathF.PI / 2);

        Transform.Rotation = Quaternion.CreateFromYawPitchRoll(_rotation.X, _rotation.Y, _rotation.Z);
    }
}
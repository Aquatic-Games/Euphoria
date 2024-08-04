using System;
using System.Numerics;
using Euphoria.Engine;
using Euphoria.Engine.Entities.Components;
using Euphoria.Engine.InputSystem;

namespace Tests.Engine.Components;

public class CameraMove : Component
{
    private Vector3 _rotation;
    
    public override void Update(float dt)
    {
        if (Input.UIWantsFocus)
            return;
        
        float speed = 5 * dt;

        if (Input.IsKeyDown(Key.W))
            Transform.Position += Transform.Forward * speed;
        if (Input.IsKeyDown(Key.S))
            Transform.Position += Transform.Backward * speed;
        if (Input.IsKeyDown(Key.A))
            Transform.Position += Transform.Left * speed;
        if (Input.IsKeyDown(Key.D))
            Transform.Position += Transform.Right * speed;

        if (Input.IsKeyDown(Key.Space))
            Transform.Position += Transform.Up * speed;
        if (Input.IsKeyDown(Key.LeftControl))
            Transform.Position += Transform.Down * speed;

        const float mouseSpeed = 0.01f;

        _rotation += new Vector3(-Input.MouseDelta * mouseSpeed, 0);
        _rotation.Y = float.Clamp(_rotation.Y, -MathF.PI / 2, MathF.PI / 2);

        Transform.Rotation = Quaternion.CreateFromYawPitchRoll(_rotation.X, _rotation.Y, _rotation.Z);
    }
}
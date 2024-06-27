using System;
using System.Numerics;
using Euphoria.Engine;
using Euphoria.Engine.Entities.Components;

namespace Tests.Engine.Components;

public class MoveComponent : Component
{
    public override void Update(float dt)
    {
        base.Update(dt);

        float speed = 50 * dt;

        if (Input.IsKeyDown(Key.Up))
            Transform.Position.Y -= speed;
        if (Input.IsKeyDown(Key.Down))
            Transform.Position.Y += speed;
        if (Input.IsKeyDown(Key.Left))
            Transform.Position.X -= speed;
        if (Input.IsKeyDown(Key.Right))
            Transform.Position.X += speed;
        
        if (Input.IsKeyPressed(Key.Space))
            Console.WriteLine("Press!");

        // TODO: Look into GRABS D3D11 input lag. Something to do with frame latency.
        if (Input.IsMouseButtonDown(MouseButton.Left))
            Transform.Position = new Vector3(Input.MousePosition, 0);
        if (Input.IsMouseButtonDown(MouseButton.Right))
            Transform.Position += new Vector3(Input.MouseDelta, 0);
        
        if (Input.IsMouseButtonPressed(MouseButton.Middle))
            Transform.Position = Vector3.Zero;
    }
}
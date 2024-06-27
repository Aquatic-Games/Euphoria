﻿using System;
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
    }
}
﻿using System.Numerics;

namespace Euphoria.Engine.InputSystem.Bindings;

public struct DirectionalBinding : IInputBinding<Vector2>
{
    public IInputBinding<float> Up;

    public IInputBinding<float> Down;

    public IInputBinding<float> Left;

    public IInputBinding<float> Right;
    
    public bool IsDown { get; }
    
    public bool IsPressed { get; }
    
    public Vector2 Value { get; private set; }

    public DirectionalBinding(IInputBinding<float> up, IInputBinding<float> down, IInputBinding<float> left, IInputBinding<float> right)
    {
        Up = up;
        Down = down;
        Left = left;
        Right = right;
    }

    public void Update()
    {
        Vector2 value = Vector2.Zero;
        const float amount = 1.0f;
        
        Up.Update();
        Down.Update();
        Left.Update();
        Right.Update();

        if (Up.IsDown)
            value.Y += amount;
        if (Down.IsDown)
            value.Y -= amount;
        if (Left.IsDown)
            value.X -= amount;
        if (Right.IsDown)
            value.X += amount;

        Value = value;
    }
}
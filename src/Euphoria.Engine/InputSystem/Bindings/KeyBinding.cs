using System;

namespace Euphoria.Engine.InputSystem.Bindings;

public struct KeyBinding : IInputBinding<float>
{
    public Key Key;
    
    public bool IsDown { get; private set; }
    
    public bool IsPressed { get; private set; }

    public float Value => IsDown ? 1 : 0;

    public KeyBinding(Key key)
    {
        Key = key;
    }

    public void Update()
    {
        IsDown = Input.IsKeyDown(Key);
        IsPressed = Input.IsKeyPressed(Key);
    }
}
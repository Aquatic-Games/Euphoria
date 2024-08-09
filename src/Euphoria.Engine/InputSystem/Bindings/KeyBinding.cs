using System;

namespace Euphoria.Engine.InputSystem.Bindings;

public struct KeyBinding : IInputBinding<float>
{
    public Key Key;

    public bool IsDown => Input.IsKeyDown(Key);

    public bool IsPressed => Input.IsKeyPressed(Key);

    public float Value => IsDown ? 1 : 0;

    public KeyBinding(Key key)
    {
        Key = key;
    }
}
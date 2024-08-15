using System;
using System.Numerics;

namespace Euphoria.Engine.InputSystem.Bindings;

public struct KeyBinding : IInputBinding
{
    public Key Key;

    public BindingType Type => BindingType.Key;
    
    public bool IsDown => Input.IsKeyDown(Key);

    public bool IsPressed => Input.IsKeyPressed(Key);

    public Vector3 Value => new Vector3(IsDown ? 1 : 0);

    public KeyBinding(Key key)
    {
        Key = key;
    }

    public string AsString()
        => Key.ToString();
}
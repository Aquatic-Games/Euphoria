using System;

namespace Euphoria.Engine.InputSystem.Bindings;

public struct KeyBinding : IInputBinding<bool>
{
    public Key Key;

    public BindingType Type => BindingType.Key;
    
    public bool IsDown => Input.IsKeyDown(Key);

    public bool IsPressed => Input.IsKeyPressed(Key);

    public bool Value => IsDown;

    public KeyBinding(Key key)
    {
        Key = key;
    }

    public string AsConfigString()
        => Key.ToString();
}
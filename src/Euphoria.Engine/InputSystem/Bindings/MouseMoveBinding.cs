using System;
using System.Numerics;

namespace Euphoria.Engine.InputSystem.Bindings;

public struct MouseMoveBinding : IInputBinding
{
    private float _sensitivity;

    public BindingType Type => BindingType.MouseMove;
    
    public bool IsDown => false;

    public bool IsPressed => false;

    public Vector3 Value
    {
        get
        {
            Vector2 mouseDelta = -Input.MouseDelta;
            //mouseDelta.Y = -mouseDelta.Y;
        
            return new Vector3(mouseDelta * _sensitivity, 0);
        }
    }

    public float Sensitivity
    {
        get => float.RadiansToDegrees(_sensitivity);
        set => _sensitivity = float.DegreesToRadians(value);
    }

    public MouseMoveBinding()
    {
        Sensitivity = 1.0f;
    }
    
    public MouseMoveBinding(float sensitivity)
    {
        Sensitivity = sensitivity;
    }

    public string AsString()
        => $"Sensitivity:{Sensitivity}";
}
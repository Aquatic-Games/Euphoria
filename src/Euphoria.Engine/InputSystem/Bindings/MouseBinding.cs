using System;
using System.Numerics;

namespace Euphoria.Engine.InputSystem.Bindings;

public struct MouseBinding : IInputBinding
{
    private float _sensitivity;

    public BindingType Type => BindingType.Mouse;
    
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

    public MouseBinding()
    {
        Sensitivity = 1.0f;
    }
    
    public MouseBinding(float sensitivity)
    {
        Sensitivity = sensitivity;
    }

    public string AsString()
        => $"Sensitivity:{Sensitivity}";
}
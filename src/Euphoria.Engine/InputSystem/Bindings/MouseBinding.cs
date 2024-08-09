using System;
using System.Numerics;

namespace Euphoria.Engine.InputSystem.Bindings;

public struct MouseBinding : IInputBinding<Vector2>
{
    private float _sensitivity;
    
    public bool IsDown => false;

    public bool IsPressed => false;
    
    public Vector2 Value { get; private set; }

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

    public void Update()
    {
        Vector2 mouseDelta = -Input.MouseDelta;
        //mouseDelta.Y = -mouseDelta.Y;
        
        Value = mouseDelta * _sensitivity;
    }
}
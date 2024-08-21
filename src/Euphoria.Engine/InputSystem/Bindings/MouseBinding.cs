using System.Numerics;

namespace Euphoria.Engine.InputSystem.Bindings;

public struct MouseBinding : IInputBinding
{
    public MouseButton Button;
    
    public BindingType Type => BindingType.Mouse;

    public bool IsDown => Input.IsMouseButtonDown(Button);

    public bool IsPressed => Input.IsMouseButtonPressed(Button);

    public Vector3 Value => new Vector3(IsDown ? 1 : 0);

    public MouseBinding(MouseButton button)
    {
        Button = button;
    }

    public string AsString()
        => Button.ToString();
}
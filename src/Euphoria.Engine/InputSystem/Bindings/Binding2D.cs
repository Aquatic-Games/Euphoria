using System.Numerics;

namespace Euphoria.Engine.InputSystem.Bindings;

public struct Binding2D<TBinding> : IInputBinding where TBinding : IInputBinding
{
    public TBinding Up;

    public TBinding Down;

    public TBinding Left;

    public TBinding Right;

    public BindingType Type => Up switch
    {
        KeyBinding k => BindingType.Key2D
    };
    
    public bool IsDown => Up.IsDown || Down.IsDown || Left.IsDown || Right.IsDown;

    public bool IsPressed => Up.IsPressed || Down.IsPressed || Left.IsPressed || Right.IsPressed;

    public Vector3 Value
    {
        get
        {
            Vector3 value = Vector3.Zero;

            value.Y = Up.Value.X - Down.Value.X;
            value.X = Right.Value.X - Left.Value.X;

            return value;
        }
    }

    public Binding2D(TBinding up, TBinding down, TBinding left, TBinding right)
    {
        Up = up;
        Down = down;
        Left = left;
        Right = right;
    }

    public string AsString()
        => $"Up:{Up.AsString()};Down:{Down.AsString()};Left:{Left.AsString()};Right:{Right.AsString()}";
}
using System.Numerics;

namespace Euphoria.Engine.InputSystem.Bindings;

public struct Binding2D<TBinding> : IInputBinding<Vector2> where TBinding : IInputBinding<bool>
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

    public Vector2 Value
    {
        get
        {
            Vector2 value = Vector2.Zero;
            const float amount = 1.0f;

            if (Up.IsDown)
                value.Y += amount;
            if (Down.IsDown)
                value.Y -= amount;
            if (Left.IsDown)
                value.X -= amount;
            if (Right.IsDown)
                value.X += amount;

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

    public string AsConfigString()
        => $"Up:{Up.AsConfigString()};Down:{Down.AsConfigString()};Left:{Left.AsConfigString()};Right:{Right.AsConfigString()}";
}
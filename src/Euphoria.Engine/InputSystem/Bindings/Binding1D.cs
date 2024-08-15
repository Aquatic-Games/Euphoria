using System;

namespace Euphoria.Engine.InputSystem.Bindings;

public struct Binding1D<TBinding> : IInputBinding<float> where TBinding : IInputBinding<bool>
{
    public TBinding Positive;

    public TBinding Negative;

    public BindingType Type => Positive switch
    {
        KeyBinding k => BindingType.Key1D,
        _ => throw new ArgumentOutOfRangeException()
    };
    
    public bool IsDown => Positive.IsDown || Negative.IsDown;

    public bool IsPressed => Positive.IsPressed || Negative.IsPressed;
    
    public float Value
    {
        get
        {
            float value = 0;
            const float amount = 1;
            if (Positive.IsDown)
                value += amount;
            if (Negative.IsDown)
                value -= amount;

            return value;
        }
    }

    public Binding1D(TBinding positive, TBinding negative)
    {
        Positive = positive;
        Negative = negative;
    }

    public string AsConfigString()
        => $"Positive:{Positive.AsConfigString()};Negative:{Negative.AsConfigString()}";
}
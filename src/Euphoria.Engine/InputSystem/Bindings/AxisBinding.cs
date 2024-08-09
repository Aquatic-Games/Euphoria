namespace Euphoria.Engine.InputSystem.Bindings;

public struct AxisBinding : IInputBinding<float>
{
    public IInputBinding<float> Positive;

    public IInputBinding<float> Negative;
    
    public bool IsDown { get; }
    
    public bool IsPressed { get; }

    public AxisBinding(IInputBinding<float> positive, IInputBinding<float> negative)
    {
        Positive = positive;
        Negative = negative;
    }

    public float Value => Positive.Value + -Negative.Value;
}
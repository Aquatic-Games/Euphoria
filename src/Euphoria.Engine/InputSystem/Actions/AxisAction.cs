using Euphoria.Engine.InputSystem.Bindings;

namespace Euphoria.Engine.InputSystem.Actions;

public class AxisAction : IInputAction
{
    public IInputBinding[] Positive;

    public IInputBinding[] Negative;

    public float Value;

    public AxisAction(IInputBinding[] positive, IInputBinding[] negative)
    {
        Positive = positive;
        Negative = negative;
    }

    public void Update()
    {
        Value = 0;
        
        foreach (IInputBinding binding in Positive)
        {
            binding.Update();
            Value += binding.Value;
        }

        foreach (IInputBinding binding in Negative)
        {
            binding.Update();
            Value -= binding.Value;
        }

        Value = float.Clamp(Value, -1, 1);
    }
}
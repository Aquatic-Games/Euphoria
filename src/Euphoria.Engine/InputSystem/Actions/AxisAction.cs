using Euphoria.Engine.InputSystem.Bindings;

namespace Euphoria.Engine.InputSystem.Actions;

public class AxisAction : IInputAction
{
    public IInputBinding[] Bindings;

    public float Value;

    public AxisAction(params IInputBinding[] bindings)
    {
        Bindings = bindings;
    }

    public void Update()
    {
        Value = 0;
        
        foreach (IInputBinding binding in Bindings)
        {
            binding.Update();
            Value += binding.Value;
        }
    }
}
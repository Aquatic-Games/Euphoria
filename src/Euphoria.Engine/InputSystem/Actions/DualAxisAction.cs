using System.Numerics;
using Euphoria.Engine.InputSystem.Bindings;

namespace Euphoria.Engine.InputSystem.Actions;

public class DualAxisAction : IInputAction
{
    public IInputBinding<Vector2>[] Bindings;

    public Vector2 Value;

    public DualAxisAction(params IInputBinding<Vector2>[] bindings)
    {
        Bindings = bindings;
    }

    public void Update()
    {
        Value = Vector2.Zero;

        foreach (IInputBinding<Vector2> binding in Bindings)
        {
            binding.Update();
            Value += binding.Value;
        }
    }
}
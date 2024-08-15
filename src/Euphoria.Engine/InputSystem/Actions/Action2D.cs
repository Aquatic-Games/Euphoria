using System.Numerics;
using Euphoria.Engine.InputSystem.Bindings;

namespace Euphoria.Engine.InputSystem.Actions;

public class Action2D : IInputAction
{
    public readonly IInputBinding<Vector2>[] Bindings;

    public Vector2 Value;

    public Action2D(params IInputBinding<Vector2>[] bindings)
    {
        Bindings = bindings;
    }

    public void Update()
    {
        Value = Vector2.Zero;

        foreach (IInputBinding<Vector2> binding in Bindings)
            Value += binding.Value;
    }
}
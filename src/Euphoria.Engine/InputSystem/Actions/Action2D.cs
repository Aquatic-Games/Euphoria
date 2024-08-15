using System;
using System.Numerics;
using System.Text;
using Euphoria.Engine.InputSystem.Bindings;

namespace Euphoria.Engine.InputSystem.Actions;

public class Action2D : InputAction
{
    public readonly IInputBinding<Vector2>[] Bindings;

    public Vector2 Value;
    
    protected override IInputBindingBase[] BaseBindings =>
        Array.ConvertAll(Bindings, binding => (IInputBindingBase) binding);

    public Action2D(params IInputBinding<Vector2>[] bindings)
    {
        Bindings = bindings;
    }

    public override void Update()
    {
        Value = Vector2.Zero;

        foreach (IInputBinding<Vector2> binding in Bindings)
            Value += binding.Value;
    }
}
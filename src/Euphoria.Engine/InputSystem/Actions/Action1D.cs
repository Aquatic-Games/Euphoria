using System;
using System.Text;
using Euphoria.Engine.InputSystem.Bindings;
using Euphoria.Parsers;

namespace Euphoria.Engine.InputSystem.Actions;

public class Action1D : InputAction
{
    public readonly IInputBinding<float>[] Bindings;

    public float Value;
    
    protected override IInputBindingBase[] BaseBindings =>
        Array.ConvertAll(Bindings, binding => (IInputBindingBase) binding);

    public Action1D(params IInputBinding<float>[] bindings)
    {
        Bindings = bindings;
    }

    public override void Update()
    {
        Value = 0;
        
        foreach (IInputBinding<float> binding in Bindings)
            Value += binding.Value;
    }
}
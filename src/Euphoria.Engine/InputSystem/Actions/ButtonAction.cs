using System;
using System.Text;
using Euphoria.Engine.InputSystem.Bindings;

namespace Euphoria.Engine.InputSystem.Actions;

public class ButtonAction : InputAction
{
    public readonly IInputBinding<bool>[] Bindings;

    public bool IsDown;
    
    public bool IsPressed;

    protected override IInputBindingBase[] BaseBindings =>
        Array.ConvertAll(Bindings, binding => (IInputBindingBase) binding);

    public ButtonAction(params IInputBinding<bool>[] bindings)
    {
        Bindings = bindings;
    }
    
    public override void Update()
    {
        IsDown = false;
        IsPressed = false;
        
        foreach (IInputBinding<bool> binding in Bindings)
        {
            if (binding.IsDown)
                IsDown = true;

            if (binding.IsPressed)
                IsPressed = true;
        }
    }
}
using Euphoria.Engine.InputSystem.Bindings;

namespace Euphoria.Engine.InputSystem.Actions;

public class ButtonAction : IInputAction
{
    public IInputBinding<bool>[] Bindings;

    public bool IsDown;
    
    public bool IsPressed;

    public ButtonAction(params IInputBinding<bool>[] bindings)
    {
        Bindings = bindings;
    }
    
    public void Update()
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
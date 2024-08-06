using System;
using Euphoria.Engine.InputSystem.Bindings;

namespace Euphoria.Engine.InputSystem.Actions;

public class ButtonAction : IInputAction
{
    public IInputBinding[] Bindings;

    public bool IsDown;

    public bool IsPressed;

    public ButtonAction(params IInputBinding[] bindings)
    {
        Bindings = bindings;
    }

    public void Update()
    {
        IsDown = false;
        IsPressed = false;
        
        foreach (IInputBinding binding in Bindings)
        {
            binding.Update();

            if (binding.IsDown)
                IsDown = true;

            if (binding.IsPressed)
                IsPressed = true;
        }
    }
}
using System.Numerics;
using Euphoria.Engine.InputSystem.Bindings;

namespace Euphoria.Engine.InputSystem.Actions;

public class DualAxisAction : IInputAction
{
    public IInputBinding[] XBindings;

    public IInputBinding[] YBindings;

    public Vector2 Value;

    public DualAxisAction(IInputBinding[] xBindings, IInputBinding[] yBindings)
    {
        XBindings = xBindings;
        YBindings = yBindings;
    }

    public void Update()
    {
        Value = Vector2.Zero;

        foreach (IInputBinding binding in XBindings)
        {
            binding.Update();
            Value.X += binding.Value;
        }

        foreach (IInputBinding binding in YBindings)
        {
            binding.Update();
            Value.Y += binding.Value;
        }
    }
}
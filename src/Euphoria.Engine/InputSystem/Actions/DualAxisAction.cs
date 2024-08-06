using System.Numerics;
using Euphoria.Engine.InputSystem.Bindings;

namespace Euphoria.Engine.InputSystem.Actions;

public class DualAxisAction : IInputAction
{
    public AxisAction X;

    public AxisAction Y;

    public Vector2 Value;

    public DualAxisAction(AxisAction x, AxisAction y)
    {
        X = x;
        Y = y;
    }

    public void Update()
    {
        X.Update();
        Y.Update();

        Value = new Vector2(X.Value, Y.Value);
    }
}
using System.Text;
using Euphoria.Engine.Configs;
using Euphoria.Engine.InputSystem.Bindings;
using Euphoria.Parsers;

namespace Euphoria.Engine.InputSystem.Actions;

public abstract class InputAction
{
    protected abstract IInputBindingBase[] BaseBindings { get; }
    
    public abstract void Update();
    
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        IInputBindingBase[] bindings = BaseBindings;

        for (int i = 0; i < bindings.Length; i++)
        {
            sb.Append('{');
            sb.Append($"Type:{bindings[i].Type};");
            sb.Append(bindings[i].AsConfigString());
            sb.Append('}');
            if (i < bindings.Length - 1)
                sb.Append(',');
        }

        if (bindings.Length > 1)
        {
            sb.Insert(0, '[');
            sb.Append(']');
        }

        return sb.ToString();
    }
}
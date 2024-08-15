using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Euphoria.Engine.InputSystem.Bindings;

namespace Euphoria.Engine.InputSystem;

public class InputAction
{
    private Vector3 _value;
    private bool _isDown;
    private bool _isPressed;
    
    public readonly List<IInputBinding> Bindings;

    public bool IsDown => _isDown;

    public bool IsPressed => _isPressed;

    public bool GetBool(float threshold = 0.5f)
        => _value.X >= threshold;

    public float GetFloat()
        => _value.X;

    public Vector2 GetVector2()
        => new Vector2(_value.X, _value.Y);

    public Vector3 GetVector3()
        => _value;

    public InputAction(params IInputBinding[] bindings)
    {
        Bindings = new List<IInputBinding>(bindings);
    }

    public InputAction(List<IInputBinding> bindings)
    {
        Bindings = bindings;
    }

    public void Update()
    {
        _value = Vector3.Zero;
        _isDown = false;
        _isPressed = false;

        foreach (IInputBinding binding in Bindings)
        {
            _value += binding.Value;

            if (binding.IsDown)
                _isDown = true;
            if (binding.IsPressed)
                _isPressed = true;
        }
    }
    
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        int i = 0;
        foreach (IInputBinding binding in Bindings)
        {
            sb.Append('{');
            sb.Append($"Type:{binding.Type};");
            sb.Append(binding.AsString());
            sb.Append('}');
            if (i++ < Bindings.Count - 1)
                sb.Append(',');
        }

        if (Bindings.Count > 1)
        {
            sb.Insert(0, '[');
            sb.Append(']');
        }

        return sb.ToString();
    }

    public static InputAction FromString(string @string)
    {
        List<IInputBinding> bindings = new List<IInputBinding>();
        
        if (@string.StartsWith('['))
        {
            string trimmedString = @string.Trim('[', ']');

            string[] splitString = trimmedString.Split(',');

            foreach (string bindString in splitString)
            {
                IInputBinding binding = IInputBinding.FromString(bindString.Trim());
                bindings.Add(binding);
            }
        }
        else
        {
            IInputBinding binding = IInputBinding.FromString(@string.Trim());
            bindings.Add(binding);
        }

        return new InputAction(bindings);
    }
}
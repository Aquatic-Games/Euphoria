using System;
using System.Numerics;

namespace Euphoria.Engine.InputSystem.Bindings;

public interface IInputBinding
{
    public BindingType Type { get; }
    
    public bool IsDown { get; }
    
    public bool IsPressed { get; }
    
    public Vector3 Value { get; }
    
    public string AsString();
    
    public static IInputBinding FromString(string @string)
    {
        string binding = @string.Trim('{', '}').Trim();

        string[] splitBinding = binding.Split(';');

        BindingType type;

        if (splitBinding[0].Trim().StartsWith("Type:"))
            type = Enum.Parse<BindingType>(splitBinding[0]["Type:".Length..], true);
        else
        {
            throw new Exception(
                "Expected 'Type:' at index 0, cannot parse. If string is present but not at index 0, move it to index 0.");
        }

        switch (type)
        {
            case BindingType.Key:
                return new KeyBinding(Enum.Parse<Key>(splitBinding[1], true));

            case BindingType.Mouse:
            {
                float sensitivity = 1;
                
                for (int i = 1; i < splitBinding.Length; i++)
                {
                    string[] config = splitBinding[i].ToLower().Split(':');
                    
                    switch (config[0].Trim().ToLower())
                    {
                        case "sensitivity":
                            sensitivity = float.Parse(config[1]);
                            break;
                    }
                }

                return new MouseBinding(sensitivity);
            }
                
            case BindingType.Key1D:
                break;
            case BindingType.Key2D:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return null;
    }
}
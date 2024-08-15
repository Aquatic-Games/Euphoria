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
        string bindString = @string.Trim('{', '}').Trim();
        string[] entries = bindString.Split(';');

        BindingType type;

        if (entries[0].Trim().ToLower().StartsWith("type:"))
            type = Enum.Parse<BindingType>(entries[0]["type:".Length..], true);
        else
        {
            throw new Exception(
                "Expected 'Type:' at index 0, cannot parse. If string is present but not at index 0, move it to index 0.");
        }

        switch (type)
        {
            case BindingType.Key:
                return new KeyBinding(Enum.Parse<Key>(entries[1], true));

            case BindingType.Mouse:
            {
                float sensitivity = 1;
                
                for (int i = 1; i < entries.Length; i++)
                {
                    string entry = entries[i].Trim();
                    (string key, string value) = GetKeyValueFromEntry(entry);
                    
                    switch (key)
                    {
                        case "sensitivity":
                            sensitivity = float.Parse(value);
                            break;
                        
                        default:
                            throw new ArgumentOutOfRangeException(nameof(value), value, null);
                    }
                }

                return new MouseBinding(sensitivity);
            }

            case BindingType.Key1D:
            {
                Key? positive = null;
                Key? negative = null;

                for (int i = 1; i < entries.Length; i++)
                {
                    string entry = entries[i].Trim();

                    (string key, string value) = GetKeyValueFromEntry(entry);

                    switch (key)
                    {
                        case "positive":
                            positive = Enum.Parse<Key>(value, true);
                            break;
                        
                        case "negative":
                            negative = Enum.Parse<Key>(value, true);
                            break;
                        
                        default:
                            throw new ArgumentOutOfRangeException(nameof(value), value, null);
                    }
                }
                
                if (positive is { } kPositive && negative is { } kNegative)
                    return new Binding1D<KeyBinding>(new KeyBinding(kPositive), new KeyBinding(kNegative));
                
                throw new Exception("Binding is of type 'Key1D' but binding(s) are missing!");
            }
            
            case BindingType.Key2D:
            {
                Key? up = null;
                Key? down = null;
                Key? left = null;
                Key? right = null;

                for (int i = 1; i < entries.Length; i++)
                {
                    string entry = entries[i].Trim();

                    (string key, string value) = GetKeyValueFromEntry(entry);

                    switch (key)
                    {
                        case "up":
                            up = Enum.Parse<Key>(value, true);
                            break;
                        
                        case "down":
                            down = Enum.Parse<Key>(value, true);
                            break;
                        
                        case "left":
                            left = Enum.Parse<Key>(value, true);
                            break;
                        
                        case "right":
                            right = Enum.Parse<Key>(value, true);
                            break;
                        
                        default:
                            throw new ArgumentOutOfRangeException(nameof(value), value, null);
                    }
                }

                if (up is { } kUp && down is { } kDown && left is { } kLeft && right is { } kRight)
                {
                    return new Binding2D<KeyBinding>(new KeyBinding(kUp), new KeyBinding(kDown), new KeyBinding(kLeft),
                        new KeyBinding(kRight));
                }

                throw new Exception("Binding is of type 'Key2D' but binding(s) are missing!");
            }
            
            default:
                throw new ArgumentOutOfRangeException();
        }

        return null;
    }

    private static (string key, string value) GetKeyValueFromEntry(string entry)
    {
        int colonIndex = entry.IndexOf(':');

        string key = entry[..colonIndex].ToLower().Trim();
        string value = entry[(colonIndex + 1)..].ToLower().Trim();

        return (key, value);
    }
}
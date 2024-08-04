namespace Euphoria.Engine.Debugging.Commands;

public readonly struct Argument
{
    public readonly ArgumentType Type;
    
    public readonly string Name;

    public Argument(ArgumentType type, string name)
    {
        Type = type;
        Name = name;
    }
}
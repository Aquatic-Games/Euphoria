namespace Euphoria.Engine.Debugging.Commands;

public interface ICommand
{
    public string Name { get; }

    public string Description { get; }
    
    public Argument[] Arguments { get; }

    public void Execute(DebugConsole console, object[] args);
}
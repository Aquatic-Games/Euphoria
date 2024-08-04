namespace Euphoria.Engine.Debugging.Commands.Builtin;

public class TestCommand : ICommand
{
    public string Name => "test";

    public string Description => "Test command.";

    public Argument[] Arguments => [];

    public void Execute(DebugConsole console, object[] args)
    {
        console.Write("Command works.");
    }
}
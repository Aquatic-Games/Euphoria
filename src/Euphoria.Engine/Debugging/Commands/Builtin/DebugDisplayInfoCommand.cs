namespace Euphoria.Engine.Debugging.Commands.Builtin;

public class DebugDisplayInfoCommand : ICommand
{
    public string Name => "dbg_dispInfo";

    public string Description => "Set debug display info level.";

    public Argument[] Arguments => [new Argument(ArgumentType.Int, "Level")];
    
    public void Execute(DebugConsole console, object[] args)
    {
        StatsTab.DebugVerbosity = (int) args[0];
        console.Write($"Set debug info level to {args[0]}");
    }
}
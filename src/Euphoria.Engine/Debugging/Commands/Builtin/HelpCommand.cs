namespace Euphoria.Engine.Debugging.Commands.Builtin;

public class HelpCommand : ICommand
{
    public string Name => "help";

    public string Description => "Shows all available commands.";

    public Argument[] Arguments => [];
    public void Execute(DebugConsole console, object[] args)
    {
        foreach ((_, ICommand command) in console.Commands)
        {
            string cmdNameTitle = command.Name + " ";
            foreach (Argument argument in command.Arguments)
                cmdNameTitle += $"<{argument.Name}:{argument.Type.ToString().ToLower()}> ";

            console.Write(cmdNameTitle);
            console.Write($"  {command.Description}");
        }
    }
}
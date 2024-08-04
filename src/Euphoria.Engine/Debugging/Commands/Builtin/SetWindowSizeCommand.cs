using Euphoria.Math;

namespace Euphoria.Engine.Debugging.Commands.Builtin;

public class SetWindowSizeCommand : ICommand
{
    public string Name => "win_setSize";

    public string Description => "Set the window size.";
    public Argument[] Arguments => [new Argument(ArgumentType.Int, "Width"), new Argument(ArgumentType.Int, "Height")];
    public void Execute(DebugConsole console, object[] args)
    {
        Window.Size = new Size<int>((int) args[0], (int) args[1]);
        console.Write($"Set window size to {args[0]}x{args[1]}");
    }
}
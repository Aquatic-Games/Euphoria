using System;

namespace Euphoria.Engine.Debugging.Commands.Builtin;

public class SetWindowFullscreenCommand : ICommand
{
    public string Name => "win_setFullscreen";

    public string Description => "Set window fullscreen.";

    public Argument[] Arguments => [new Argument(ArgumentType.String, "FullscreenMode")];
    
    public void Execute(DebugConsole console, object[] args)
    {
        if (!Enum.TryParse((string) args[0], out FullscreenMode mode))
        {
            console.Write(
                $"Invalid fullscreen mode. Accepted values: {string.Join(", ", Enum.GetValues<FullscreenMode>())}");
            return;
        }

        Window.FullscreenMode = mode;
        
        console.Write($"Set window fullscreen to {mode}.");
    }
}
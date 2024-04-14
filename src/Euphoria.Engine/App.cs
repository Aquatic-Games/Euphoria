using System;
using Euphoria.Render;
using Euphoria.Render.GL;
using u4.Core;

namespace u4.Engine;

public static class App
{
    public static Window Window;
    public static Graphics Graphics;

    public static bool IsRunning;

    public static void Run(in LaunchOptions options)
    {
        Logger.Info($"{options.AppName} v{options.AppVersion}");
        Logger.Info("Starting up.");
        
        Logger.Trace("Creating window.");
        Window = new Window(options);
        Window.CloseRequested += () => IsRunning = false;

        Logger.Debug($"Selected API: {options.API}");
        
        switch (options.API)
        {
            case RenderAPI.OpenGL:
                Logger.Trace("Creating GL context.");
                Window.CreateGLContext(out Action<int> presentFunc, out Func<string, nint> getProcAddressFunc);
                
                Logger.Trace("Creating GL graphics.");
                Graphics = new GLGraphics(getProcAddressFunc, presentFunc);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Logger.Trace("Entering main loop.");
        
        IsRunning = true;
        while (IsRunning)
        {
            Window.ProcessEvents();
            
            Graphics.Present();
        }
    }
}
using System;
using Euphoria.Render;
using grabs.Graphics;
using grabs.Graphics.GL43;
using u4.Core;
using u4.Engine.Exceptions;
using Mutex = System.Threading.Mutex;

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

        // EE = Euphoria Engine
        using Mutex lockMut = new Mutex(true, $"Global\\EE-{options.AppName}", out bool createdNew);

        if (!createdNew)
            throw new MultipleInstanceException(options.AppName);

        Logger.Trace("Creating window.");
        Window = new Window(options);
        Window.CloseRequested += () => IsRunning = false;

        Logger.Debug($"Selected API: {options.Api}");
        
        switch (options.Api)
        {
            case GraphicsApi.OpenGL:
                Logger.Trace("Creating GL context.");
                Window.CreateGLContext(out Action<int> presentFunc, out Func<string, nint> getProcAddressFunc);
                
                Logger.Trace("Creating GL graphics.");
                Graphics = new Graphics(new GL43Instance(getProcAddressFunc), new GL43Surface(presentFunc),
                    Window.SizeInPixels);
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
using System;
using System.Numerics;
using System.Reflection;
using Euphoria.Render;
using grabs.Graphics;
using grabs.Graphics.D3D11;
using grabs.Graphics.GL43;
using u4.Core;
using u4.Engine.Exceptions;
using Mutex = System.Threading.Mutex;
using Texture = Euphoria.Render.Texture;

namespace u4.Engine;

public static class App
{
    public static Application Application;
    
    public static Window Window;
    public static Graphics Graphics;

    public static bool IsRunning;

    public static void Run(in LaunchOptions options, Application application = null)
    {
        Logger.Info($"{options.AppName} v{options.AppVersion}");
        Logger.Info("Starting up.");

        // EE = Euphoria Engine
        using Mutex lockMut = new Mutex(true, $"Global\\EE-{options.AppName}", out bool createdNew);

        if (!createdNew)
            throw new MultipleInstanceException(options.AppName);

        Application = application ?? new Application();

        Logger.Trace("Creating window.");
        Window = new Window(options);
        Window.CloseRequested += () => IsRunning = false;

        Logger.Debug($"Selected API: {options.Api}");
        string releaseConfigName = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration ?? "";
        Window.EngineTitle = $" v{options.AppVersion} {releaseConfigName.ToUpper()} - {options.Api}";
        
        switch (options.Api)
        {
            case GraphicsApi.D3D11:
                Logger.Trace("Creating D3D11 graphics.");
                Graphics = new Graphics(new D3D11Instance(), new D3D11Surface(Window.Hwnd), Window.SizeInPixels, options.GraphicsOptions);
                break;
            
            case GraphicsApi.OpenGL:
                Logger.Trace("Creating GL context.");
                Window.CreateGLContext(out Action<int> presentFunc, out Func<string, nint> getProcAddressFunc);
                
                Logger.Trace("Creating GL graphics.");
                Graphics = new Graphics(new GL43Instance(getProcAddressFunc), new GL43Surface(presentFunc),
                    Window.SizeInPixels, options.GraphicsOptions);
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        Logger.Debug("Initializing user code.");
        Application.Initialize();

        Logger.Trace("Entering main loop.");
        
        IsRunning = true;
        while (IsRunning)
        {
            Window.ProcessEvents();
            
            Application.Update(1f / 60f);
            Application.Draw();
            
            Graphics.Present();
        }
        
        Graphics.Dispose();
        Window.Dispose();
    }
}
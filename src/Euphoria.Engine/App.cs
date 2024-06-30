using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Euphoria.Core;
using Euphoria.Engine.Exceptions;
using Euphoria.Engine.Scenes;
using Euphoria.Render;
using grabs.Graphics;
using grabs.Graphics.D3D11;
using grabs.Graphics.GL43;
using Mutex = System.Threading.Mutex;

namespace Euphoria.Engine;

public static class App
{
    private static double _targetDelta;
    private static int _targetFps;
    
    public static string Name { get; private set; }
    public static Version Version { get; private set; }
    public static string ReleaseConfig { get; private set; }
    
    public static bool IsRunning { get; private set; }

    public static int TargetFramesPerSecond
    {
        get => _targetFps;
        set
        {
            _targetFps = value;
            if (_targetFps == 0)
                _targetDelta = 0;
            else
                _targetDelta = 1.0 / _targetFps;
        }
    }
    
    public static Application Application { get; private set; }
    
    public static Window Window { get; private set; }
    public static Graphics Graphics { get; private set; }

    static App()
    {
        ConsoleColor currentColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("    #####  #   #  #####  #   #  #####  #####  #####  #####");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("   #      #   #  #   #  #   #  #   #  #   #    #    #   #       =---------------------=");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("  #####  #   #  #####  #####  #   #  # ###    #    #####        | Euphoria Engine 0.1 |");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(" #      #   #  #      #   #  #   #  #  #     #    #   #         |  Aquatic Games 2024 |");
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("#####  #####  #      #   #  #####  #   #  #####  #   #          =---------------------=");

        Console.ForegroundColor = currentColor;
    }

    public static void Run(in LaunchOptions options, Scene initialScene, Application application = null)
    {
        Name = options.AppName;
        Version = options.AppVersion;
        ReleaseConfig = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration.ToUpper() ?? "";
        
        Logger.Debug("Starting application.");
        Logger.Info($"Application: {options.AppName} v{options.AppVersion}, release config: {ReleaseConfig}");

        // EE = Euphoria Engine
        using Mutex lockMut = new Mutex(true, $"Global\\EE-{options.AppName}", out bool createdNew);

        if (!createdNew)
        {
            MessageBox.Show(MessageBox.Type.Error, options.AppName, $"An instance of {options.AppName} is already running.");
            throw new MultipleInstanceException(options.AppName);
        }

        Application = application ?? new Application();

        Logger.Trace("Creating window.");
        Window = new Window(options);
        Window.CloseRequested += () => IsRunning = false;

        Logger.Debug($"Selected API: {options.Api}");
        
        switch (options.Api)
        {
            case GraphicsApi.D3D11:
                Logger.Trace("Creating D3D11 graphics.");
                Graphics = new Graphics(new D3D11Instance(), new D3D11Surface(Window.Hwnd), Window.SizeInPixels,
                    options.GraphicsOptions);
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
        
        SetEngineTitle(0);
        
        Logger.Debug("Initializing metrics system.");
        Metrics.Initialize(SetEngineTitle);
        
        Logger.Debug("Initializing input system.");
        Input.Initialize(Window);

        ImGuiController.Initialize(Graphics, Window);
        
        Logger.Debug("Initializing user code.");
        Application.Initialize(initialScene);

        Logger.Trace("Entering main loop.");
        
        IsRunning = true;
        while (IsRunning)
        {
            if (Metrics.Update(_targetDelta))
                continue;
            
            Input.Update();
            Window.ProcessEvents();

            float dt = (float) Metrics.TimeSinceLastFrame;
            
            ImGuiController.Update(dt);
            
            Application.Update(dt);
            Application.Draw();
            
            Graphics.Present();
        }
        
        Application.Dispose();
        Graphics.Dispose();
        Window.Dispose();
    }

    public static void Close()
    {
        IsRunning = false;
    }

    public static GraphicsApi ShowGraphicsApiSelector(bool exitOnCancel = true)
    {
        Logger.Debug("Showing graphics API selection window.");
        
        List<MessageBox.Button> buttons = new List<MessageBox.Button>();
        StringBuilder message = new StringBuilder("Select graphics API:\n");

        if (IsGraphicsApiSupported(GraphicsApi.D3D11))
        {
            buttons.Add(new MessageBox.Button("DirectX 11", (int) GraphicsApi.D3D11));
            message.AppendLine("- DirectX 11: Recommended for most systems.");
        }
        
        if (IsGraphicsApiSupported(GraphicsApi.Vulkan))
        {
            buttons.Add(new MessageBox.Button("Vulkan", (int) GraphicsApi.Vulkan));
            message.AppendLine("- Vulkan: Experimental. May produce better FPS but may be less stable.");
        }
        
        if (IsGraphicsApiSupported(GraphicsApi.OpenGL))
        {
            buttons.Add(new MessageBox.Button("OpenGL", (int) GraphicsApi.OpenGL));
            message.AppendLine("- OpenGL: Legacy. Use if no other options work.");
        }
        
        buttons.Add(new MessageBox.Button("Auto", int.MaxValue));
        message.Append("Press auto if you don't know which one to pick.");
        
        buttons.Add(new MessageBox.Button("Cancel", -1));

        int value = MessageBox.Show(MessageBox.Type.None, "Select API", message.ToString(), buttons.ToArray());
        
        if (value == -1 && exitOnCancel)
            Environment.Exit(0);
        else if (value == int.MaxValue)
            value = (int) PickBestGraphicsApi();

        return (GraphicsApi) value;
    }

    public static GraphicsApi PickBestGraphicsApi()
    {
        if (IsGraphicsApiSupported(GraphicsApi.D3D11))
            return GraphicsApi.D3D11;

        if (IsGraphicsApiSupported(GraphicsApi.OpenGL))
            return GraphicsApi.OpenGL;

        if (IsGraphicsApiSupported(GraphicsApi.OpenGLES))
            return GraphicsApi.OpenGLES;

        throw new Exception("No Graphics API is supported.");
    }

    public static bool IsGraphicsApiSupported(GraphicsApi api)
    {
        switch (api)
        {
            case GraphicsApi.D3D11:
                if (OperatingSystem.IsWindows())
                    return true;

                break;
                
            case GraphicsApi.OpenGL:
                if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD())
                    return true;

                break;
                
            case GraphicsApi.OpenGLES: // OpenGL ES is also not currently supported.
                //if (!OperatingSystem.IsMacOS())
                //    return true;

                break;
            
            case GraphicsApi.Vulkan: // Vulkan is not supported right now.
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(api), api, null);
        }

        return false;
    }

    private static void SetEngineTitle(int fps)
    {
        Window.EngineTitle = $" v{Version} {ReleaseConfig} | {Graphics.Api} | {fps} FPS";
    }
}
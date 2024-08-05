using System;
using Euphoria.Engine.Configs;
using Euphoria.Math;
using Euphoria.Render;
using grabs.Graphics;

namespace Euphoria.Engine;

public struct LaunchOptions
{
    public string AppName;

    public Version AppVersion;

    public WindowInfo Window;
    
    public GraphicsInfo Graphics;

    public int TargetFramesPerSecond;

    public int TargetTicksPerSecond;

    public LaunchOptions(string appName, Version appVersion)
    {
        AppName = appName;
        AppVersion = appVersion;

        Window = WindowInfo.Default with { Title = appName };
        Graphics = GraphicsInfo.Default;

        TargetFramesPerSecond = 0;
        TargetTicksPerSecond = 60;
    }

    public static LaunchOptions FromConfig(EuphoriaConfig config, string appName, Version appVersion)
    {
        LaunchOptions options = new LaunchOptions(appName, appVersion);
        options.Window.Size = config.Display.Size;
        options.Window.FullscreenMode = config.Display.FullscreenMode;

        options.Graphics.Api = config.Graphics.Api;
        options.Graphics.AdapterIndex = config.Graphics.Adapter;

        return options;
    }
}
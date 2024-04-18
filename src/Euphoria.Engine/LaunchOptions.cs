using System;
using Euphoria.Render;
using grabs.Graphics;
using u4.Math;

namespace u4.Engine;

public struct LaunchOptions
{
    public string AppName;

    public Version AppVersion;
    
    public string WindowTitle;
    
    // TODO: Euphoria.Math.Size<T>
    public Size<int> WindowSize;

    public GraphicsApi Api;

    public LaunchOptions(string appName, Version appVersion)
    {
        AppName = appName;
        AppVersion = appVersion;

        WindowTitle = appName;
        WindowSize = new Size<int>(1280, 720);

        if (OperatingSystem.IsWindows())
            Api = GraphicsApi.D3D11;
        else
            Api = GraphicsApi.OpenGL;
    }
}
using System;
using System.Drawing;
using Euphoria.Render;

namespace u4.Engine;

public struct LaunchOptions
{
    public string AppName;

    public Version AppVersion;
    
    public string WindowTitle;
    
    // TODO: Euphoria.Math.Size<T>
    public Size WindowSize;

    public RenderAPI API;

    public LaunchOptions(string appName, Version appVersion)
    {
        AppName = appName;
        AppVersion = appVersion;

        WindowTitle = appName;
        WindowSize = new Size(1280, 720);
        API = RenderAPI.OpenGL;
    }
}
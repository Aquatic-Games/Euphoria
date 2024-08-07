﻿using System;
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

    public void ApplyConfig(EuphoriaConfig config)
    {
        if (config.Display is { } display)
        {
            Window.Size = display.Size ?? Window.Size;
            Window.FullscreenMode = display.FullscreenMode ?? Window.FullscreenMode;
        }

        if (config.Graphics is { } graphics)
        {
            Graphics.Api = graphics.Api ?? Graphics.Api;
            Graphics.AdapterIndex = graphics.Adapter ?? Graphics.AdapterIndex;
        }
    }
}
﻿using System;
using Euphoria.Math;
using Euphoria.Render;
using grabs.Graphics;

namespace Euphoria.Engine;

public struct LaunchOptions
{
    public string AppName;

    public Version AppVersion;
    
    public string WindowTitle;
    
    public Size<int> WindowSize;

    public bool Resizable;

    public GraphicsApi Api;

    public GraphicsOptions GraphicsOptions;

    public LaunchOptions(string appName, Version appVersion)
    {
        AppName = appName;
        AppVersion = appVersion;

        WindowTitle = appName;
        WindowSize = new Size<int>(1280, 720);
        Resizable = false;

        Api = App.PickBestGraphicsApi();
        
        GraphicsOptions = GraphicsOptions.Default;
    }
}
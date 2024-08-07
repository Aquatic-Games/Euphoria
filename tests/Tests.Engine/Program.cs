﻿using System;
using Euphoria.Core;
using Euphoria.Engine;
using Euphoria.Engine.Configs;
using grabs.Graphics;
using Tests.Engine;
using Tests.Engine.Scenes;

Logger.AttachConsole();

LaunchOptions options = new LaunchOptions("EuphoriaTests", new Version(0, 1));
options.Window.Border = WindowBorder.Resizable;

if (EuphoriaConfig.TryLoadFromFile("Config.cfg", out EuphoriaConfig config))
{
    EuphoriaConfig.CurrentConfig = config;
    options.ApplyConfig(config);
}
else
{
    Logger.Warn("Config does not exist, using default settings.");
    
    GraphicsApi api = App.ShowGraphicsApiSelector();
    options.Graphics.Api = api;
}

App.Run(options, new PhysicsScene(), new TestApp());
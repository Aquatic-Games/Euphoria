using System;
using Euphoria.Core;
using Euphoria.Engine;
using Euphoria.Engine.Configs;
using grabs.Graphics;
using Tests.Engine;
using Tests.Engine.Scenes;

Logger.AttachConsole();

LaunchOptions options;

if (EuphoriaConfig.TryLoadFromFile("Config.cfg", out EuphoriaConfig config))
{
    EuphoriaConfig.CurrentConfig = config;
    options = LaunchOptions.FromConfig(config, "EuphoriaTests", new Version(0, 1));
    options.Window.Border = WindowBorder.Resizable;
}
else
{
    Logger.Warn("Config does not exist, using default settings.");
    
    GraphicsApi api = App.ShowGraphicsApiSelector();
    
    options = new LaunchOptions("EuphoriaTests", new Version(0, 1))
    {
        Window = WindowInfo.Default with { Border = WindowBorder.Resizable },
        Graphics = GraphicsInfo.DefaultWithApi(api) /* with { AdapterIndex = 1 }*/
    };
}

App.Run(options, new PhysicsScene(), new TestApp());
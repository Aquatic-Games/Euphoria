using System;
using Euphoria.Core;
using Euphoria.Engine;
using grabs.Graphics;
using Tests.Engine;
using Tests.Engine.Scenes;

Logger.AttachConsole();

GraphicsApi api = App.ShowGraphicsApiSelector();

LaunchOptions options = new LaunchOptions("EuphoriaTests", new Version(0, 1))
{
    WindowBorder = WindowBorder.Resizable,
    Api = api
};

App.Run(options, new TestScene(), new TestApp());
﻿using System;
using Euphoria.Core;
using Euphoria.Engine;
using grabs.Graphics;
using Tests.Engine;
using Tests.Engine.Scenes;

Logger.AttachConsole();

GraphicsApi api = App.ShowGraphicsApiSelector();

LaunchOptions options = new LaunchOptions("EuphoriaTests", new Version(0, 1))
{
    Window = WindowInfo.Default with { Border = WindowBorder.Resizable },
    Graphics = GraphicsInfo.DefaultWithApi(api)/* with { AdapterIndex = 1 }*/
};

App.Run(options, new PhysicsScene(), new TestApp());
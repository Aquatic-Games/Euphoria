using System;
using Euphoria.Core;
using Euphoria.Engine;

Logger.AttachConsole();

LaunchOptions options = new LaunchOptions("EuphoriaTests", new Version(0, 1));

App.Run(options);
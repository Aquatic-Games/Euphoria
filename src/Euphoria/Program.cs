using System;
using u4.Core;
using u4.Engine;
using u4.Engine.Exceptions;

Logger.AttachConsole();

Logger.Trace("Trace message.");
Logger.Debug("Debug message.");
Logger.Info("Info message.");
Logger.Warn("Warning message");
Logger.Error("Error message.");
Logger.Fatal("Fatal message.");

LaunchOptions options = new LaunchOptions("Test", new Version(1, 0));

try
{
    App.Run(options);
}
catch (MultipleInstanceException e)
{
    MessageBox.Show(MessageBox.Type.Error, $"{options.WindowTitle} error", e.Message);
}
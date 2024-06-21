using System;
using Silk.NET.SDL;

namespace Euphoria.Engine;

public static class MessageBox
{
    public enum Type
    {
        None,
        Information,
        Warning,
        Error
    }

    public static unsafe void Show(Type type, string title, string message)
    {
        using Sdl sdl = Sdl.GetApi();

        MessageBoxFlags flags = type switch
        {
            Type.None => MessageBoxFlags.None,
            Type.Information => MessageBoxFlags.Information,
            Type.Warning => MessageBoxFlags.Warning,
            Type.Error => MessageBoxFlags.Error,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        sdl.ShowSimpleMessageBox((uint)flags, title, message, null);
    }
}
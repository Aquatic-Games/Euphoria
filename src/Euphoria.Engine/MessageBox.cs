using System;
using grabs.Core;
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

    public static unsafe int Show(Type type, string title, string message, Button[] buttons)
    {
        using PinnedString pTitle = new PinnedString(title);
        using PinnedString pMessage = new PinnedString(message);

        PinnedString[] buttonTexts = new PinnedString[buttons.Length];
        MessageBoxButtonData* buttonDatas = stackalloc MessageBoxButtonData[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            buttonTexts[i] = new PinnedString(buttons[i].Text);

            buttonDatas[i] = new MessageBoxButtonData()
            {
                Buttonid = buttons[i].Id,
                Text = buttonTexts[i]
            };
        }

        MessageBoxFlags flags = MessageBoxFlags.ButtonsLeftToRight;
        flags |= type switch
        {
            Type.None => MessageBoxFlags.None,
            Type.Information => MessageBoxFlags.Information,
            Type.Warning => MessageBoxFlags.Warning,
            Type.Error => MessageBoxFlags.Error,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        MessageBoxData mBox = new MessageBoxData()
        {
            Flags = (uint) flags,
            Window = null,
            Title = pTitle,
            Message = pMessage,

            Numbuttons = buttons.Length,
            Buttons = buttonDatas
        };
        
        using Sdl sdl = Sdl.GetApi();
        int buttonId;
        sdl.ShowMessageBox(&mBox, &buttonId);
        
        for (int i = 0; i < buttons.Length; i++)
            buttonTexts[i].Dispose();

        return buttonId;
    }

    public readonly struct Button
    {
        public readonly string Text;
        public readonly int Id;

        public Button(string text, int id)
        {
            Text = text;
            Id = id;
        }
    }
}
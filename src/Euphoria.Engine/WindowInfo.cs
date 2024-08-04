using Euphoria.Math;

namespace Euphoria.Engine;

public struct WindowInfo
{
    public Size<int> Size;

    public string Title;

    public WindowBorder Border;

    public FullscreenMode FullscreenMode;

    public WindowInfo(Size<int> size, string title, WindowBorder border, FullscreenMode fullscreenMode)
    {
        Size = size;
        Title = title;
        Border = border;
        FullscreenMode = fullscreenMode;
    }

    public static WindowInfo Default => new()
    {
        Size = new Size<int>(1280, 720),
        Title = "Euphoria Window",
        Border = WindowBorder.Fixed,
        FullscreenMode = FullscreenMode.Windowed
    };

    public static WindowInfo DefaultFullscreen => Default with { FullscreenMode = FullscreenMode.Borderless };
}
using Euphoria.Math;

namespace Euphoria.Engine;

public struct WindowInfo
{
    public Size<int> Size;

    public string Title;

    public WindowBorder Border;

    public WindowInfo(Size<int> size, string title, WindowBorder border)
    {
        Size = size;
        Title = title;
        Border = border;
    }

    public static WindowInfo Default => new()
    {
        Size = new Size<int>(1280, 720),
        Title = "Euphoria Window",
        Border = WindowBorder.Fixed
    };
}
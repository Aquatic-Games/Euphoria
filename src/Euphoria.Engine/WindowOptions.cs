using System.Drawing;
using u4.Graphics;

namespace u4.Engine;

public struct WindowOptions
{
    // TODO: Euphoria.Math.Size
    public Size Size;

    public string Title;

    public GraphicsAPI Api;
}
using System.Numerics;
using System.Runtime.InteropServices;

namespace Euphoria.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Rectangle<T> where T : INumber<T>
{
    public Vector2T<T> Position;
    
    public Size<T> Size;

    public Rectangle(Vector2T<T> position, Size<T> size)
    {
        Position = position;
        Size = size;
    }
    
    public T X
    {
        get => Position.X;
        set => Position.X = value;
    }

    public T Y
    {
        get => Position.Y;
        set => Position.Y = value;
    }

    public T Width
    {
        get => Size.Width;
        set => Size.Width = value;
    }

    public T Height
    {
        get => Size.Height;
        set => Size.Height = value;
    }

    public Rectangle(T x, T y, T width, T height)
    {
        Position = new Vector2T<T>(x, y);
        Size = new Size<T>(width, height);
    }

    public readonly Rectangle<TOther> As<TOther>() where TOther : INumber<TOther>
        => new Rectangle<TOther>(Position.As<TOther>(), Size.As<TOther>());
}
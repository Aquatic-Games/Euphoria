using System.Numerics;

namespace Euphoria.Math;

public struct Size<T> where T : INumber<T>
{
    public T Width;
    
    public T Height;

    public Size(T width, T height)
    {
        Width = width;
        Height = height;
    }

    public Size(T wh)
    {
        Width = wh;
        Height = wh;
    }

    public readonly Size<TOther> As<TOther>() where TOther : INumber<TOther>
        => new Size<TOther>(TOther.CreateChecked(Width), TOther.CreateChecked(Height));

    public override string ToString()
    {
        return $"{Width}x{Height}";
    }
}
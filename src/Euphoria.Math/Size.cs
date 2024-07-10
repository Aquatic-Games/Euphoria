using System;
using System.Collections.Generic;
using System.Numerics;

namespace Euphoria.Math;

public struct Size<T> : IEquatable<Size<T>> where T : INumber<T>
{
    public static Size<T> Zero => new Size<T>(T.Zero); 
    
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

    public static bool operator ==(Size<T> left, Size<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Size<T> left, Size<T> right)
    {
        return !left.Equals(right);
    }
    
    public override string ToString()
    {
        return $"{Width}x{Height}";
    }

    public bool Equals(Size<T> other)
    {
        return Width == other.Width && Height == other.Height;
    }

    public override bool Equals(object obj)
    {
        return obj is Size<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }
}
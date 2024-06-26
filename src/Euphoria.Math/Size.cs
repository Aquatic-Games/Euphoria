﻿namespace Euphoria.Math;

public struct Size<T>
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

    public override string ToString()
    {
        return $"{Width}x{Height}";
    }
}
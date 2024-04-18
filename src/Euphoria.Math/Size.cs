namespace u4.Math;

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
}
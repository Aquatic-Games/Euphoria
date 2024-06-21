using System.Numerics;

namespace Euphoria.Math;

public struct Vector2T<T> where T : INumber<T>
{
    public static Vector2T<T> Zero => new Vector2T<T>(T.Zero);
    
    public static Vector2T<T> One => new Vector2T<T>(T.One);

    public static Vector2T<T> UnitX => new Vector2T<T>(T.One, T.Zero);

    public static Vector2T<T> UnitY => new Vector2T<T>(T.Zero, T.One);
    
    public T X;

    public T Y;

    public Vector2T(T scalar)
    {
        X = scalar;
        Y = scalar;
    }

    public Vector2T(T x, T y)
    {
        X = x;
        Y = y;
    }
}
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Euphoria.Math;

[StructLayout(LayoutKind.Sequential)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector2T<TOther> As<TOther>() where TOther : INumber<TOther>
        => new Vector2T<TOther>(TOther.CreateChecked(X), TOther.CreateChecked(Y));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2T<T> operator +(in Vector2T<T> left, in Vector2T<T> right)
        => new Vector2T<T>(left.X + right.X, left.Y + right.Y);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2T<T> operator -(in Vector2T<T> left, in Vector2T<T> right)
        => new Vector2T<T>(left.X - right.X, left.Y - right.Y);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2T<T> operator *(in Vector2T<T> left, in Vector2T<T> right)
        => new Vector2T<T>(left.X * right.X, left.Y * right.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2T<T> operator *(in Vector2T<T> left, T right)
        => new Vector2T<T>(left.X * right, left.Y * right);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2T<T> operator /(in Vector2T<T> left, in Vector2T<T> right)
        => new Vector2T<T>(left.X / right.X, left.Y / right.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2T<T> operator /(in Vector2T<T> left, T right)
        => new Vector2T<T>(left.X / right, left.Y / right);
    
    
}
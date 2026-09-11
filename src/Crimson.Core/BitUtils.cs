using System.Runtime.CompilerServices;

namespace Crimson.Core;

/// <summary>
/// Contains utilities for bitwise operations.
/// </summary>
public static class BitUtils
{
    /// <summary>
    /// Round a uint to the next power of 2.
    /// </summary>
    /// <param name="value">The uint to round.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint RoundToNextPowerOf2(uint value)
    {
        // https://graphics.stanford.edu/%7Eseander/bithacks.html#RoundUpPowerOf2
        value--;
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;
        return ++value;
    }
}
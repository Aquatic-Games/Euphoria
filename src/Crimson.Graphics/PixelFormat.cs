namespace Crimson.Graphics;

/// <summary>
/// Represents various supported pixel formats.
/// </summary>
public enum PixelFormat
{
    /// <summary>
    /// 8-bit RGBA (32bpp)
    /// </summary>
    RGBA8
}

public static class PixelFormatExtensions
{
    extension(PixelFormat fmt)
    {
        public uint BytesPerPixel => fmt switch
        {
            PixelFormat.RGBA8 => 4,
            _ => throw new ArgumentOutOfRangeException(nameof(fmt), fmt, null)
        };

        public uint BitsPerPixel => fmt.BytesPerPixel * 8;
    }
}
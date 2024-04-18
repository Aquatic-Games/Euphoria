using System.IO;
using grabs.Graphics;
using StbImageSharp;
using u4.Math;

namespace Euphoria.Render;

public class Bitmap
{
    public byte[] Data;

    public Size<int> Size;

    public Format Format;

    public Bitmap(string path)
    {
        using FileStream stream = File.OpenRead(path);
        ImageResult result = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

        Data = result.Data;
        Size = new Size<int>(result.Width, result.Height);
        Format = Format.R8G8B8A8_UNorm;
    }
    
    public Bitmap(byte[] data, Size<int> size, Format format)
    {
        Data = data;
        Size = size;
        Format = format;
    }
}
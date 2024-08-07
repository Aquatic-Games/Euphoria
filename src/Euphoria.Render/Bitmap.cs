using System.IO;
using Euphoria.Core;
using Euphoria.Math;
using grabs.Graphics;
using StbImageSharp;

namespace Euphoria.Render;

public class Bitmap
{
    public byte[] Data;

    public Size<int> Size;

    public Format Format;

    public Bitmap(string path)
    {
        Logger.Trace($"Loading image \"{path}\".");
        
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
using System;
using Euphoria.Math;
// TODO: grabs.Text!
using Pie.Text;

namespace Euphoria.Render.Text;

public class Font : IDisposable
{
    private FontFace _face;
    
    public Font(string path)
    {
        _face = new FontFace(path, new Size<int>(2048, 1024));
    }
    
    public void Dispose()
    {
        _face.Dispose();
    }

    internal static FreeType FreeType;

    static Font()
    {
        FreeType = new FreeType();
    }
}
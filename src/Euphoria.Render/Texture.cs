using System;
using u4.Math;

namespace Euphoria.Render;

public class Texture : IDisposable
{
    internal readonly GrabsTexture GTexture;

    public readonly Size<int> Size;

    internal Texture(GrabsTexture gTexture, Size<int> size)
    {
        GTexture = gTexture;
        Size = size;
    }

    public void Dispose()
    {
        GTexture.Dispose();
    }
}
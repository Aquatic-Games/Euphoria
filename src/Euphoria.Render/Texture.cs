using System;

namespace Euphoria.Render;

public class Texture : IDisposable
{
    public GrabsTexture ApiTexture;

    public Texture(GrabsTexture apiTexture)
    {
        ApiTexture = apiTexture;
    }

    public void Dispose()
    {
        ApiTexture.Dispose();
    }
}
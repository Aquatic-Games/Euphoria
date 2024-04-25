using System;
using grabs.Graphics;
using u4.Math;

namespace Euphoria.Render.Renderers;

public sealed class Renderer2D : IDisposable
{
    internal Framebuffer Framebuffer;
    internal GrabsTexture ColorTexture;

    public Renderer2D(Device device, Size<int> size)
    {
        ColorTexture = device.CreateTexture(TextureDescription.Texture2D((uint) size.Width, (uint) size.Height, 1,
            Format.R8G8B8A8_UNorm, TextureUsage.Framebuffer | TextureUsage.ShaderResource));

        Framebuffer = device.CreateFramebuffer(new ReadOnlySpan<GrabsTexture>(ref ColorTexture));
        
        
    }
    
    public void Dispose()
    {
        Framebuffer.Dispose();
        ColorTexture.Dispose();
    }
}
using System;
using grabs.Graphics;
using u4.Math;

namespace Euphoria.Render.Renderers;

public class Renderer3D : IDisposable
{
    private Framebuffer _gBuffer;
    private GrabsTexture _albedoTexture;
    
    public CameraInfo Camera;

    public Renderer3D(Device device, Size<int> size)
    {
        
    }

    public void Dispose()
    {
        
    }
}
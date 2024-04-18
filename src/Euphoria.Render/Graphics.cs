using System;
using grabs.Graphics;

namespace Euphoria.Render;

public sealed class Graphics : IDisposable
{
    private Swapchain _swapchain;
    private Texture _swapchainTexture;
    private Framebuffer _swapchainBuffer;
    
    public Instance Instance;
    public Device Device;
    public CommandList CommandList;

    public Graphics(Instance instance, Surface surface)
    {
        
    }
    
    public void Present()
    {
        _swapchain.Present();
    }

    public void Dispose()
    {
        
    }
}
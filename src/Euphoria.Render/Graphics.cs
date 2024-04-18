using System;
using grabs.Graphics;
using u4.Math;

namespace Euphoria.Render;

public sealed class Graphics : IDisposable
{
    private Swapchain _swapchain;
    private Texture _swapchainTexture;
    private Framebuffer _swapchainBuffer;
    
    public Instance Instance;
    public Device Device;
    public CommandList CommandList;

    public VSyncMode VSyncMode
    {
        get
        {
            return _swapchain.PresentMode switch
            {
                PresentMode.Immediate => VSyncMode.Off,
                PresentMode.VerticalSync => VSyncMode.VSync,
                PresentMode.AdaptiveSync => VSyncMode.Adaptive,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        set
        {
            _swapchain.PresentMode = value switch
            {
                VSyncMode.Off => PresentMode.Immediate,
                VSyncMode.VSync => PresentMode.VerticalSync,
                VSyncMode.Adaptive => PresentMode.AdaptiveSync,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }
    }

    public Graphics(Instance instance, Surface surface, Size<int> size, GraphicsOptions options, Adapter? adapter = null)
    {
        Instance = instance;
        Device = Instance.CreateDevice(adapter);

        _swapchain = Device.CreateSwapchain(surface,
            new SwapchainDescription((uint) size.Width, (uint) size.Height, presentMode: PresentMode.VerticalSync));
        _swapchainTexture = _swapchain.GetSwapchainTexture();

        _swapchainBuffer = Device.CreateFramebuffer(new ReadOnlySpan<Texture>(ref _swapchainTexture));

        CommandList = Device.CreateCommandList();
    }
    
    public void Present()
    {
        _swapchain.Present();
    }

    public void Dispose()
    {
        CommandList.Dispose();
        _swapchainBuffer.Dispose();
        _swapchainTexture.Dispose();
        _swapchain.Dispose();
        Device.Dispose();
        Instance.Dispose();
    }
}
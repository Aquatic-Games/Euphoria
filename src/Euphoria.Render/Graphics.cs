using System;
using Euphoria.Render.Renderers;
using grabs.Graphics;
using u4.Core;
using u4.Math;

namespace Euphoria.Render;

public sealed class Graphics : IDisposable
{
    private readonly Swapchain _swapchain;
    private readonly Texture _swapchainTexture;
    private readonly Framebuffer _swapchainBuffer;
    
    public readonly Instance Instance;
    public readonly Device Device;
    public readonly CommandList CommandList;

    public readonly TextureBatcher TextureBatcher;

    public readonly Renderer2D Renderer2D;
    public readonly Renderer3D Renderer3D;

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
        
        Logger.Trace("Creating texture renderer.");
        TextureBatcher = new TextureBatcher(Device, CommandList);
        
        Logger.Debug($"Render type: {options.RenderType}");

        switch (options.RenderType)
        {
            case RenderType.None:
                break;
            case RenderType.Only2D:
                Renderer2D = new Renderer2D();
                break;
            case RenderType.Only3D:
                Renderer3D = new Renderer3D();
                break;
            case RenderType.Both:
                Renderer2D = new Renderer2D();
                Renderer3D = new Renderer3D();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public void Present()
    {
        _swapchain.Present();
    }

    public void Dispose()
    {
        TextureBatcher.Dispose();
        
        CommandList.Dispose();
        _swapchainBuffer.Dispose();
        _swapchainTexture.Dispose();
        _swapchain.Dispose();
        Device.Dispose();
        Instance.Dispose();
    }
}
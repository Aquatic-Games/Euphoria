global using GrabsTexture = grabs.Graphics.Texture;

using System;
using System.Numerics;
using Euphoria.Render.Renderers;
using grabs.Graphics;
using u4.Core;
using u4.Math;
using Buffer = grabs.Graphics.Buffer;

namespace Euphoria.Render;

public sealed class Graphics : IDisposable
{
    private readonly Swapchain _swapchain;
    private readonly GrabsTexture _swapchainTexture;
    private readonly Framebuffer _swapchainBuffer;

    private Size<int> _size;
    
    internal readonly Instance Instance;
    internal readonly Device Device;
    internal readonly CommandList CommandList;

    internal readonly ShaderLoader ShaderLoader;

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

    public Graphics(Instance instance, Surface surface, Size<int> size, GraphicsOptions options, ShaderLoader loader, Adapter? adapter = null)
    {
        Instance = instance;
        _size = size;
        ShaderLoader = loader;

        // TOO MANY ADAPTERS
        Adapter[] adapters = Instance.EnumerateAdapters();
        Adapter currentAdapter = adapters[adapter?.Index ?? 0];
        Logger.Debug($"EnumerateAdapters:\n    {string.Join('\n', adapters).Replace("\n", "\n    ")}");
        Logger.Info($"Using adapter {currentAdapter.Name}");
        
        Device = Instance.CreateDevice(surface, adapter);

        _swapchain = Device.CreateSwapchain(new SwapchainDescription((uint) size.Width, (uint) size.Height,
            presentMode: PresentMode.VerticalSync));
        _swapchainTexture = _swapchain.GetSwapchainTexture();

        _swapchainBuffer = Device.CreateFramebuffer(new ReadOnlySpan<GrabsTexture>(ref _swapchainTexture));

        CommandList = Device.CreateCommandList();
        
        Logger.Trace("Creating texture renderer.");
        TextureBatcher = new TextureBatcher(Device, ShaderLoader);
        
        Logger.Debug($"Render type: {options.RenderType}");

        switch (options.RenderType)
        {
            case RenderType.None:
                break;
            case RenderType.Only2D:
                Logger.Trace("Creating 2D renderer.");
                Renderer2D = new Renderer2D(Device, size, ShaderLoader);
                break;
            case RenderType.Only3D:
                Logger.Trace("Creating 3D renderer.");
                Renderer3D = new Renderer3D(Device, size, ShaderLoader);
                break;
            case RenderType.Both:
                Logger.Trace("Creating 2D renderer.");
                Renderer2D = new Renderer2D(Device, size, ShaderLoader);
                
                Logger.Trace("Creating 3D renderer.");
                Renderer3D = new Renderer3D(Device, size, ShaderLoader);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public Texture CreateTexture(Bitmap bitmap)
    {
        GrabsTexture texture =
            Device.CreateTexture(
                TextureDescription.Texture2D((uint) bitmap.Size.Width, (uint) bitmap.Size.Height, 0, bitmap.Format,
                    TextureUsage.ShaderResource), new ReadOnlySpan<byte>(bitmap.Data));
        
        // TODO: Mipmaps queue to be done in present.
        CommandList.Begin();
        CommandList.GenerateMipmaps(texture);
        CommandList.End();
        Device.ExecuteCommandList(CommandList);

        return new Texture(texture, bitmap.Size);
    }

    public Renderable CreateRenderable(Mesh mesh)
    {
        Buffer vertexBuffer = Device.CreateBuffer(BufferType.Vertex, mesh.Vertices);
        Buffer indexBuffer = Device.CreateBuffer(BufferType.Index, mesh.Indices);

        return new Renderable(vertexBuffer, indexBuffer, (uint) mesh.Indices.Length);
    }
    
    public void Present()
    {
        CommandList.Begin();
        CommandList.SetViewport(new Viewport(0, 0, (uint) _size.Width, (uint) _size.Height));
        
        Renderer2D?.DispatchRender(Device, CommandList, _swapchainBuffer);
        
        CommandList.BeginRenderPass(new RenderPassDescription(_swapchainBuffer, Vector4.Zero, LoadOp.Load));
        TextureBatcher.DispatchDrawQueue(Device, CommandList, _size);
        CommandList.EndRenderPass();
        
        CommandList.End();
        Device.ExecuteCommandList(CommandList);
        
        _swapchain.Present();
    }

    public void Dispose()
    {
        Renderer3D?.Dispose();
        Renderer2D?.Dispose();
        TextureBatcher.Dispose();
        
        CommandList.Dispose();
        _swapchainBuffer.Dispose();
        _swapchainTexture.Dispose();
        _swapchain.Dispose();
        Device.Dispose();
        Instance.Dispose();
    }
}
global using GrabsTexture = grabs.Graphics.Texture;

using System;
using System.Drawing;
using System.Numerics;
using Euphoria.Core;
using Euphoria.Math;
using Euphoria.Render.Renderers;
using grabs.Graphics;
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

    internal ItemIdCollection<Texture> Textures;

    public GraphicsApi Api => Instance.Api;

    public readonly TextureBatcher TextureBatcher;

    //public readonly Renderer2D Renderer2D;
    public readonly Renderer3D Renderer3D;
    
    public readonly ImGuiRenderer ImGuiRenderer;

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
        _size = size;

        Textures = new ItemIdCollection<Texture>();

        // TOO MANY ADAPTERS
        Adapter[] adapters = Instance.EnumerateAdapters();
        Adapter currentAdapter = adapters[adapter?.Index ?? 0];
        Logger.Debug($"EnumerateAdapters:\n    {string.Join('\n', adapters).Replace("\n", "\n    ")}");
        Logger.Info($"Using adapter {currentAdapter.Name}");
        
        Logger.Trace("Creating device.");
        Device = Instance.CreateDevice(surface, adapter);

        Logger.Trace("Creating swapchain.");
        _swapchain = Device.CreateSwapchain(new SwapchainDescription((uint) size.Width, (uint) size.Height,
            presentMode: PresentMode.VerticalSync, bufferCount: 2));
        
        _swapchainTexture = _swapchain.GetSwapchainTexture();

        Logger.Trace("Creating swapchain buffer.");
        _swapchainBuffer = Device.CreateFramebuffer(new ReadOnlySpan<GrabsTexture>(ref _swapchainTexture));

        Logger.Trace("Creating main command list.");
        CommandList = Device.CreateCommandList();
        
        Logger.Trace("Creating texture renderer.");
        TextureBatcher = new TextureBatcher(Device);
        
        Logger.Debug($"Render type: {options.RenderType}");
        // TODO: Implement render type, and 2D renderer, which is currently disabled to aid development of the 3D renderer.
        Logger.Warn("Currently the render type is being IGNORED. This will be implemented in a later version.");
        
        Logger.Trace("Creating 3D renderer.");
        Renderer3D = new Renderer3D(Device, size);
        
        Logger.Trace("Creating IMGUI renderer.");
        ImGuiRenderer = new ImGuiRenderer(Device);

        /*switch (options.RenderType)
        {
            case RenderType.None:
                break;
            case RenderType.Only2D:
                Logger.Trace("Creating 2D renderer.");
                Renderer2D = new Renderer2D(Device, size);
                break;
            case RenderType.Only3D:
                Logger.Trace("Creating 3D renderer.");
                Renderer3D = new Renderer3D(Device, size);
                break;
            case RenderType.Both:
                Logger.Trace("Creating 2D renderer.");
                Renderer2D = new Renderer2D(Device, size);

                Logger.Trace("Creating 3D renderer.");
                Renderer3D = new Renderer3D(Device, size);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }*/
    }

    public Texture CreateTexture(Bitmap bitmap)
    {
        GrabsTexture texture =
            Device.CreateTexture(
                TextureDescription.Texture2D((uint) bitmap.Size.Width, (uint) bitmap.Size.Height, 0, bitmap.Format,
                    TextureUsage.ShaderResource | TextureUsage.GenerateMips), bitmap.Data);

        DescriptorSet descriptorSet = Device.CreateDescriptorSet(TextureBatcher.TextureDescriptorLayout,
            new DescriptorSetDescription(texture: texture));
        
        // TODO: Mipmaps queue to be done in present.
        CommandList.Begin();
        CommandList.GenerateMipmaps(texture);
        CommandList.End();
        Device.ExecuteCommandList(CommandList);

        ulong id = Textures.NextId;
        Texture tex = new Texture(this, texture, descriptorSet, id, bitmap.Size);
        Textures.AddItem(tex);

        return tex;
    }
    
    public void Present()
    {
        CommandList.Begin();
        CommandList.SetViewport(new Viewport(0, 0, (uint) _size.Width, (uint) _size.Height));
        CommandList.SetScissor(new Rectangle(0, 0, _size.Width, _size.Height));
        
        Renderer3D.Render(CommandList, _swapchainBuffer);
        
        //Renderer2D?.DispatchRender(Device, CommandList, _swapchainBuffer);
        
        // TODO: UI Renderer instead of texture batcher.
        CommandList.BeginRenderPass(new RenderPassDescription(_swapchainBuffer, Vector4.Zero, LoadOp.Load));
        TextureBatcher.DispatchDrawQueue(CommandList, _size);
        CommandList.EndRenderPass();
        
        ImGuiRenderer.Render(CommandList, _swapchainBuffer, Textures);
        
        CommandList.End();
        Device.ExecuteCommandList(CommandList);
        
        _swapchain.Present();
    }

    public void Dispose()
    {
        ImGuiRenderer.Dispose();
        Renderer3D?.Dispose();
        //Renderer2D?.Dispose();
        TextureBatcher.Dispose();
        
        CommandList.Dispose();
        _swapchainBuffer.Dispose();
        _swapchainTexture.Dispose();
        _swapchain.Dispose();
        Device.Dispose();
        Instance.Dispose();
    }
}
global using GrabsTexture = grabs.Graphics.Texture;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Euphoria.Core;
using Euphoria.Math;
using Euphoria.Render.Renderers;
using grabs.Graphics;
using Buffer = grabs.Graphics.Buffer;

namespace Euphoria.Render;

public static class Graphics
{
    private static Size<int> _size;
    
    internal static Instance Instance;
    internal static Device Device;
    internal static CommandList CommandList;
    
    private static Swapchain _swapchain;
    private static GrabsTexture _swapchainTexture;
    internal static Framebuffer SwapchainFramebuffer;

    internal static DescriptorLayout TextureDescriptorLayout;

    internal static List<GrabsTexture> TexturesQueuedForMipGeneration;

    public static RenderType RenderType { get; private set; }
    
    public static TextureBatcher TextureBatcher { get; private set; }
    
    public static Renderer3D Renderer3D { get; private set; }
    
    public static ImGuiRenderer ImGuiRenderer { get; private set; }

    public static GraphicsApi Api => Instance.Api;

    public static Size<int> Size => _size;

    public static VSyncMode VSyncMode
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

    public static void Initialize(Instance instance, Surface surface, Size<int> size, GraphicsOptions options, Adapter? adapter = null)
    {
        Instance = instance;
        _size = size;

        TexturesQueuedForMipGeneration = new List<GrabsTexture>();

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
        SwapchainFramebuffer = Device.CreateFramebuffer(new ReadOnlySpan<GrabsTexture>(ref _swapchainTexture));
        
        Logger.Trace("Creating texture descriptor layout.");
        TextureDescriptorLayout = Device.CreateDescriptorLayout(new DescriptorLayoutDescription(
            new DescriptorBindingDescription(0, DescriptorType.Texture, ShaderStage.Pixel)));

        Logger.Trace("Creating main command list.");
        CommandList = Device.CreateCommandList();
        
        Logger.Trace("Creating texture renderer.");
        TextureBatcher = new TextureBatcher(Device);
        
        Logger.Trace("Creating IMGUI renderer.");
        ImGuiRenderer = new ImGuiRenderer(Device, size);
        
        Logger.Debug($"Render type: {options.RenderType}");
        RenderType = options.RenderType;

        if (options.RenderType == RenderType.Normal)
        {
            Logger.Trace("Creating 3D renderer.");
            Renderer3D = new Renderer3D(Device, size);
        }
    }

    public static Cubemap CreateCubemap(Bitmap right, Bitmap left, Bitmap top, Bitmap bottom, Bitmap front, Bitmap back)
    {
        TextureDescription textureDesc = TextureDescription.Cubemap((uint) right.Size.Width, (uint) right.Size.Height,
            1, Format.R8G8B8A8_UNorm, TextureUsage.ShaderResource);

        GrabsTexture texture = Device.CreateTexture(textureDesc,
            [right.Data, left.Data, top.Data, bottom.Data, front.Data, back.Data]);
        
        DescriptorSet descriptorSet = Device.CreateDescriptorSet(TextureBatcher.TextureDescriptorLayout,
            new DescriptorSetDescription(texture: texture));

        return new Cubemap(texture, descriptorSet, right.Size);
    }
    
    public static void Present()
    {
        CommandList.Begin();
        CommandList.SetViewport(new Viewport(0, 0, (uint) _size.Width, (uint) _size.Height));
        CommandList.SetScissor(new Rectangle(0, 0, _size.Width, _size.Height));
        
        // Mips generation queue. Generate mipmaps for all textures that have requested it.
        // The reason this is not in the Texture class is because it applies to ALL textures. Cubemaps, render targets, etc.
        foreach (GrabsTexture texture in TexturesQueuedForMipGeneration)
            CommandList.GenerateMipmaps(texture);
        
        TexturesQueuedForMipGeneration.Clear();
        
        Renderer3D?.Render(CommandList, SwapchainFramebuffer, _size);
        
        //Renderer2D?.DispatchRender(Device, CommandList, _swapchainBuffer);
        
        // TODO: UI Renderer instead of texture batcher.
        RenderPassDescription description = new RenderPassDescription(SwapchainFramebuffer, Vector4.Zero, LoadOp.Load);
        if (RenderType == RenderType.UIOnly)
        {
            description.ClearColor = new Vector4(0, 0, 0, 1);
            description.ColorLoadOp = LoadOp.Clear;
        }
        
        CommandList.BeginRenderPass(description);
        TextureBatcher.DispatchDrawQueue(CommandList, _size);
        CommandList.EndRenderPass();
        
        ImGuiRenderer.Render(CommandList, SwapchainFramebuffer);
        
        CommandList.End();
        Device.ExecuteCommandList(CommandList);
        
        _swapchain.Present();
    }

    public static void Resize(in Size<int> size)
    {
        _size = size;
        
        Logger.Debug($"Graphics resize requested to {size}");
        
        Logger.Trace("Disposing old resources.");
        SwapchainFramebuffer.Dispose();
        _swapchainTexture.Dispose();
        
        Logger.Trace("Resizing swapchain.");
        _swapchain.Resize((uint) size.Width, (uint) size.Height);
        
        Logger.Trace("Creating new resources.");
        _swapchainTexture = _swapchain.GetSwapchainTexture();
        SwapchainFramebuffer = Device.CreateFramebuffer(_swapchainTexture);
        
        Logger.Trace("Resizing renderers.");
        Renderer3D.Resize(size);
        ImGuiRenderer.Resize(size);
    }

    public static void Deinitialize()
    {
        Renderer3D?.Dispose();
        ImGuiRenderer.Dispose();
        TextureBatcher.Dispose();
        
        Texture.DisposeAllTextures();
        
        TextureDescriptorLayout.Dispose();
        
        CommandList.Dispose();
        SwapchainFramebuffer.Dispose();
        _swapchainTexture.Dispose();
        _swapchain.Dispose();
        Device.Dispose();
        Instance.Dispose();
    }
}
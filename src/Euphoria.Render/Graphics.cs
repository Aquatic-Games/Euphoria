global using GrabsTexture = grabs.Graphics.Texture;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Euphoria.Core;
using Euphoria.Math;
using Euphoria.Render.Renderers;
using Euphoria.Render.Text;
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
    internal static Sampler DefaultSampler;

    internal static HashSet<GrabsTexture> TexturesQueuedForMipGeneration;

    public static RenderType RenderType { get; private set; }
    
    public static TextureBatcher TextureBatcher { get; private set; }
    
    public static Renderer3D Renderer3D { get; private set; }
    
    public static ImGuiRenderer ImGuiRenderer { get; private set; }

    public static GraphicsApi Api => Instance.Api;
    
    public static Adapter Adapter { get; private set; }

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

    public static void Create(Instance instance, Surface surface, Size<int> size, GraphicsSettings settings, int adapterIndex = 0)
    {
        Instance = instance;
        _size = size;

        TexturesQueuedForMipGeneration = new HashSet<GrabsTexture>();
        
        Adapter[] adapters = Instance.EnumerateAdapters();
        Logger.Debug($"EnumerateAdapters:\n    {string.Join('\n', adapters).Replace("\n", "\n    ")}");
        
        Logger.Info($"Selected adapter index: {adapterIndex}");

        if (adapterIndex >= adapters.Length)
        {
            Logger.Warn($"Adapter index was {adapterIndex}, but only {adapters.Length} adapters are present. The value has been set to 0.");
            adapterIndex = 0;
        }

        Adapter adapter = adapters[adapterIndex];
        Adapter = adapter;
        Logger.Info($"Using adapter '{adapter.Name}'");
        
        Logger.Trace("Creating device.");
        Device = Instance.CreateDevice(surface, adapter);

        Logger.Trace("Creating swapchain.");
        _swapchain = Device.CreateSwapchain(new SwapchainDescription((uint) size.Width, (uint) size.Height,
            presentMode: PresentMode.VerticalSync, bufferCount: 2));
        
        _swapchainTexture = _swapchain.GetSwapchainTexture();

        Logger.Trace("Creating swapchain buffer.");
        SwapchainFramebuffer = Device.CreateFramebuffer(new ReadOnlySpan<GrabsTexture>(ref _swapchainTexture));
        
        Logger.Trace("Creating default sampler.");
        DefaultSampler = Device.CreateSampler(SamplerDescription.LinearClamp);
        
        Logger.Trace("Creating texture descriptor layout.");
        TextureDescriptorLayout = Device.CreateDescriptorLayout(new DescriptorLayoutDescription(
            new DescriptorBindingDescription(0, DescriptorType.Texture, ShaderStage.Pixel)));

        Logger.Trace("Creating main command list.");
        CommandList = Device.CreateCommandList();
        
        Logger.Trace("Creating texture renderer.");
        TextureBatcher = new TextureBatcher(Device);
        
        Logger.Trace("Creating IMGUI renderer.");
        ImGuiRenderer = new ImGuiRenderer(Device, size);
        
        Logger.Debug($"Render type: {settings.RenderType}");
        RenderType = settings.RenderType;

        if (settings.RenderType == RenderType.Normal)
        {
            Logger.Trace("Creating 3D renderer.");
            Renderer3D = new Renderer3D(Device, size);
        }
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

    public static Adapter[] GetAdapters()
        => Instance.EnumerateAdapters();

    public static void Destroy()
    {
        Renderer3D?.Dispose();
        ImGuiRenderer.Dispose();
        TextureBatcher.Dispose();
        
        Font.DisposeAllFonts();
        // TODO: This seems to cause an access violation exception if no text is rendered. Not sure why.
        //Font.FreeType.Dispose();
        Material.DisposeAllMaterials();
        Texture.DisposeAllTextures();
        
        DefaultSampler.Dispose();
        TextureDescriptorLayout.Dispose();
        
        CommandList.Dispose();
        SwapchainFramebuffer.Dispose();
        _swapchainTexture.Dispose();
        _swapchain.Dispose();
        Device.Dispose();
        Instance.Dispose();
    }
}
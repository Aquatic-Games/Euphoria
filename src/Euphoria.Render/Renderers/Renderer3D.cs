using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Euphoria.Core;
using Euphoria.Math;
using Euphoria.Render.Primitives;
using Euphoria.Render.Renderers.Structs;
using grabs.Graphics;
using Buffer = grabs.Graphics.Buffer;
using Color = Euphoria.Math.Color;

namespace Euphoria.Render.Renderers;

public class Renderer3D : IDisposable
{
    private readonly Device _device;
    private Size<int> _size;
    
    private readonly List<TransformedRenderable> _opaques;
    
    private GrabsTexture _depthTexture;
    
    private GrabsTexture _albedoTexture;
    private GrabsTexture _positionTexture;
    private GrabsTexture _normalTexture;
    private GrabsTexture _metallicRoughnessTexture;
    private Framebuffer _gBuffer;

    private GrabsTexture _passTexture;
    private Framebuffer _passBuffer;
    
    internal readonly ShaderModule GBufferVertexModule;
    internal readonly ShaderModule GBufferPixelModule;
    internal readonly InputLayoutDescription[] GBufferInputLayout;

    internal readonly DescriptorLayout CameraInfoLayout;
    internal readonly DescriptorLayout DrawInfoLayout;
    internal readonly DescriptorLayout MaterialInfoLayout;
    
    private readonly Pipeline _passPipeline;
    private readonly Pipeline _compositePipeline;
    
    private readonly Buffer _cameraInfoBuffer;
    private readonly DescriptorSet _cameraInfoSet;
    
    private readonly Buffer _drawInfoBuffer;
    private readonly DescriptorSet _drawInfoSet;

    private readonly DescriptorSet _passInputSet;
    private readonly DescriptorSet _compositeSet;

    private readonly Buffer _skyboxVertexBuffer;
    private readonly Buffer _skyboxIndexBuffer;
    private readonly Pipeline _skyboxPipeline;

    // Only created if requested.
    private (string, Texture)[] _debugTextures;
    
    public CameraInfo Camera;

    public Color BackgroundColor;

    public Cubemap Skybox;

    public Renderer3D(Device device, Size<int> size)
    {
        _device = device;
        _size = size;
        _opaques = new List<TransformedRenderable>();
        
        CreateTextureResources();

        Logger.Trace("Loading GBuffer shaders.");
        
        GBufferVertexModule = device.CreateShaderModule(ShaderStage.Vertex,
            ShaderLoader.LoadSpirvShader("Deferred/GBuffer", ShaderStage.Vertex), "VSMain");
        GBufferPixelModule = device.CreateShaderModule(ShaderStage.Pixel,
            ShaderLoader.LoadSpirvShader("Deferred/GBuffer", ShaderStage.Pixel), "PSMain");

        Logger.Trace("Creating GBuffer layouts.");
        
        CameraInfoLayout = device.CreateDescriptorLayout(
            new DescriptorLayoutDescription(new DescriptorBindingDescription(0, DescriptorType.ConstantBuffer,
                ShaderStage.Vertex)));
        DrawInfoLayout = device.CreateDescriptorLayout(
            new DescriptorLayoutDescription(new DescriptorBindingDescription(0, DescriptorType.ConstantBuffer,
                ShaderStage.Vertex)));
        
        MaterialInfoLayout = device.CreateDescriptorLayout(
            new DescriptorLayoutDescription(
                new DescriptorBindingDescription(0, DescriptorType.Texture, ShaderStage.Pixel), // Albedo
                new DescriptorBindingDescription(1, DescriptorType.Texture, ShaderStage.Pixel), // Normal
                new DescriptorBindingDescription(2, DescriptorType.Texture, ShaderStage.Pixel), // Metallic
                new DescriptorBindingDescription(3, DescriptorType.Texture, ShaderStage.Pixel), // Roughness
                new DescriptorBindingDescription(4, DescriptorType.Texture, ShaderStage.Pixel) // Occlusion
                ));

        GBufferInputLayout =
        [
            new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex), // Position
            new InputLayoutDescription(Format.R32G32_Float, 12, 0, InputType.PerVertex), // TexCoord
            new InputLayoutDescription(Format.R32G32B32A32_Float, 20, 0, InputType.PerVertex), // Color
            new InputLayoutDescription(Format.R32G32B32_Float, 36, 0, InputType.PerVertex), // Normal
            new InputLayoutDescription(Format.R32G32B32_Float, 48, 0, InputType.PerVertex) // Tangent
        ];

        Logger.Trace("Creating GBuffer buffers.");
        
        _cameraInfoBuffer = device.CreateBuffer(BufferType.Constant, Camera, true);
        _cameraInfoSet =
            device.CreateDescriptorSet(CameraInfoLayout, new DescriptorSetDescription(buffer: _cameraInfoBuffer));
        
        _drawInfoBuffer = device.CreateBuffer(BufferType.Constant, Matrix4x4.Identity, true);
        _drawInfoSet =
            device.CreateDescriptorSet(DrawInfoLayout, new DescriptorSetDescription(buffer: _drawInfoBuffer));

        Logger.Trace("Loading pass shaders.");

        // We need to flip the framebuffers upside down if we're using opengl, because opengl is stupid.
        bool isOpenGL = Graphics.Api is GraphicsApi.OpenGL or GraphicsApi.OpenGLES;

        using ShaderModule passVertex = device.CreateShaderModule(ShaderStage.Vertex,
            ShaderLoader.LoadSpirvShader("QuadDraw", ShaderStage.Vertex), "VSMain",
            [new SpecializationConstant(0, isOpenGL)]);
        using ShaderModule passPixel = device.CreateShaderModule(ShaderStage.Pixel,
            ShaderLoader.LoadSpirvShader("Deferred/LightingPass", ShaderStage.Pixel), "PSMain");

        Logger.Trace("Creating pass layout.");
        
        using DescriptorLayout passInputLayout = device.CreateDescriptorLayout(
            new DescriptorLayoutDescription(
                new DescriptorBindingDescription(0, DescriptorType.Texture, ShaderStage.Pixel), // Albedo
                new DescriptorBindingDescription(1, DescriptorType.Texture, ShaderStage.Pixel), // Position
                new DescriptorBindingDescription(2, DescriptorType.Texture, ShaderStage.Pixel), // Normal
                new DescriptorBindingDescription(3, DescriptorType.Texture, ShaderStage.Pixel) // MetallicRoughness
                )
            );

        Logger.Trace("Creating pass pipeline.");

        PipelineDescription passPipelineDesc = new PipelineDescription(passVertex, passPixel, null,
            DepthStencilDescription.Disabled, RasterizerDescription.CullCounterClockwise, BlendDescription.Disabled,
            [passInputLayout]);

        _passPipeline = device.CreatePipeline(passPipelineDesc);

        Logger.Trace("Creating pass descriptors.");

        _passInputSet = device.CreateDescriptorSet(passInputLayout,
            new DescriptorSetDescription(texture: _albedoTexture),
            new DescriptorSetDescription(texture: _positionTexture),
            new DescriptorSetDescription(texture: _normalTexture),
            new DescriptorSetDescription(texture: _metallicRoughnessTexture));

        Logger.Trace("Loading composite shaders.");
        
        using ShaderModule compositePixel = device.CreateShaderModule(ShaderStage.Pixel,
            ShaderLoader.LoadSpirvShader("Render/Composite", ShaderStage.Pixel), "PSMain");

        Logger.Trace("Creating composite layout.");
        
        using DescriptorLayout compositeLayout = device.CreateDescriptorLayout(
            new DescriptorLayoutDescription(new DescriptorBindingDescription(0, DescriptorType.Texture,
                ShaderStage.Pixel)));

        Logger.Trace("Creating composite pipeline.");

        PipelineDescription compositePipelineDesc = new PipelineDescription(passVertex, compositePixel, null,
            DepthStencilDescription.Disabled, RasterizerDescription.CullCounterClockwise, BlendDescription.Disabled,
            [compositeLayout]);

        _compositePipeline = device.CreatePipeline(compositePipelineDesc);

        Logger.Trace("Creating composite descriptors.");
        
        _compositeSet =
            device.CreateDescriptorSet(compositeLayout, new DescriptorSetDescription(texture: _passTexture));
        
        Logger.Trace("Creating skybox buffers.");
        
        Cube cube = new Cube();

        _skyboxVertexBuffer = device.CreateBuffer(BufferType.Vertex, cube.Vertices);
        _skyboxIndexBuffer = device.CreateBuffer(BufferType.Index, cube.Indices);
        
        Logger.Trace("Loading skybox shaders.");

        using ShaderModule skyboxVertex = device.CreateShaderModule(ShaderStage.Vertex,
            ShaderLoader.LoadSpirvShader("Skybox/Skybox", ShaderStage.Vertex), "VSMain");
        using ShaderModule skyboxPixel = device.CreateShaderModule(ShaderStage.Pixel,
            ShaderLoader.LoadSpirvShader("Skybox/Skybox", ShaderStage.Pixel), "PSMain");
        
        Logger.Trace("Creating skybox layouts.");

        using DescriptorLayout skyboxTextureLayout = device.CreateDescriptorLayout(
            new DescriptorLayoutDescription(new DescriptorBindingDescription(0, DescriptorType.Texture,
                ShaderStage.Pixel)));
        
        Logger.Trace("Creating skybox pipeline.");

        InputLayoutDescription[] skyboxInputLayout =
            [new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex)];

        PipelineDescription skyboxDesc = new PipelineDescription(skyboxVertex, skyboxPixel, skyboxInputLayout,
            DepthStencilDescription.DepthLessEqual, RasterizerDescription.CullCounterClockwise, BlendDescription.Disabled,
            [CameraInfoLayout, skyboxTextureLayout]);

        _skyboxPipeline = device.CreatePipeline(skyboxDesc);
        
        BackgroundColor = Color.Black;
        Skybox = null;
    }

    public void Draw(Renderable renderable, in Matrix4x4 world)
    {
        _opaques.Add(new TransformedRenderable(renderable, world));
    }

    public (string, Texture)[] GetDebugTextures()
    {
        if (_debugTextures == null)
        {
            Logger.Debug("Creating debug textures.");

            _debugTextures =
            [
                ("Pass", new Texture(_passTexture, _size, false)), 
                ("Albedo", new Texture(_albedoTexture, _size, false)),
                ("Position", new Texture(_positionTexture, _size, false)),
                ("Normal", new Texture(_normalTexture, _size, false)),
                ("MetallicRoughness", new Texture(_metallicRoughnessTexture, _size, false))
            ];
        }

        return _debugTextures;
    }

    internal void Render(CommandList cl, Framebuffer swapchainBuffer, Size<int> framebufferSize)
    {
        cl.UpdateBuffer(_cameraInfoBuffer, 0, Camera);
        cl.SetDescriptorSet(0, _cameraInfoSet);
        
        cl.SetViewport(new Viewport(0, 0, (uint) _size.Width, (uint) _size.Height));
        cl.SetScissor(new Rectangle(0, 0, _size.Width, _size.Height));
        
        RenderPassDescription gBufferPassDesc = new RenderPassDescription(_gBuffer, Vector4.Zero);
        cl.BeginRenderPass(gBufferPassDesc);

        foreach (TransformedRenderable tRenderable in _opaques)
        {
            Renderable renderable = tRenderable.Renderable;
            Material material = renderable.Material;
            
            cl.UpdateBuffer(_drawInfoBuffer, 0, tRenderable.Transform);
            cl.SetDescriptorSet(1, _drawInfoSet);
            cl.SetDescriptorSet(2, material.MatDescriptor);
            
            cl.SetPipeline(material.Pipeline);
            
            cl.SetVertexBuffer(0, renderable.VertexBuffer, Vertex.SizeInBytes, 0);
            cl.SetIndexBuffer(renderable.IndexBuffer, Format.R32_UInt);
            
            // TODO: Draw non-indexed.
            cl.DrawIndexed(renderable.NumElements);
        }
        
        cl.EndRenderPass();

        RenderPassDescription lightingPassDesc = new RenderPassDescription(_passBuffer, (Vector4) BackgroundColor, depthLoadOp: LoadOp.Load);
        cl.BeginRenderPass(lightingPassDesc);
        
        cl.SetDescriptorSet(0, _passInputSet);
        cl.SetPipeline(_passPipeline);
        
        cl.Draw(6);

        // Draw skybox.
        if (Skybox != null)
        {
            cl.SetPipeline(_skyboxPipeline);
            cl.SetDescriptorSet(0, _cameraInfoSet);
            cl.SetDescriptorSet(1, Skybox.DescriptorSet);

            cl.SetVertexBuffer(0, _skyboxVertexBuffer, Vertex.SizeInBytes, 0);
            cl.SetIndexBuffer(_skyboxIndexBuffer, Format.R32_UInt);

            cl.DrawIndexed(36);
        }

        cl.EndRenderPass();

        cl.SetViewport(new Viewport(0, 0, (uint) framebufferSize.Width, (uint) framebufferSize.Height));
        cl.SetScissor(new Rectangle(0, 0, framebufferSize.Width, framebufferSize.Height));
        
        RenderPassDescription compositePassDesc = new RenderPassDescription(swapchainBuffer, new Vector4(0, 0, 0, 1));
        cl.BeginRenderPass(compositePassDesc);
        
        cl.SetPipeline(_compositePipeline);
        cl.SetDescriptorSet(0, _compositeSet);
        
        cl.Draw(6);
        
        cl.EndRenderPass();
        
        // TODO: Don't clear here, have a "NewFrame" method or something. Would allow support for multiple cameras.
        _opaques.Clear();
    }

    internal void Resize(in Size<int> size)
    {
        Logger.Trace($"Resizing to {size}.");
        _size = size;
        
        DisposeTextureResources();
        CreateTextureResources();
        UpdateDescriptorResources();
    }

    private void CreateTextureResources()
    {
        Logger.Trace("Creating texture resources.");
        
        TextureDescription textureDesc = TextureDescription.Texture2D((uint) _size.Width, (uint) _size.Height, 1,
            Format.D32_Float, TextureUsage.None);
        
        // TODO GRABS doesn't allow a TextureUsage of Framebuffer if creating a depth texture, D3D11 crashes - perhaps this should be fixed?
        _depthTexture = _device.CreateTexture(textureDesc);
        
        textureDesc.Format = Format.R32G32B32A32_Float;
        textureDesc.Usage = TextureUsage.Framebuffer | TextureUsage.ShaderResource;
        
        Logger.Trace("Creating GBuffer.");

        _albedoTexture = _device.CreateTexture(textureDesc);
        _positionTexture = _device.CreateTexture(textureDesc);
        _normalTexture = _device.CreateTexture(textureDesc);
        _metallicRoughnessTexture = _device.CreateTexture(textureDesc);
        _gBuffer = _device.CreateFramebuffer(
            [_albedoTexture, _positionTexture, _normalTexture, _metallicRoughnessTexture], _depthTexture);
        
        Logger.Trace("Creating pass buffer.");
        
        _passTexture = _device.CreateTexture(textureDesc);
        _passBuffer = _device.CreateFramebuffer([_passTexture], _depthTexture);
    }

    private void UpdateDescriptorResources()
    {
        // TODO: Probably can move the initializing in the constructor to here as well - just create the descriptor sets with nothing in them.
        
        Logger.Trace("Updating descriptor resources.");

        _device.UpdateDescriptorSet(_passInputSet, 
            new DescriptorSetDescription(texture: _albedoTexture),
            new DescriptorSetDescription(texture: _positionTexture),
            new DescriptorSetDescription(texture: _normalTexture),
            new DescriptorSetDescription(texture: _metallicRoughnessTexture));

        _device.UpdateDescriptorSet(_compositeSet, new DescriptorSetDescription(texture: _passTexture));
    }

    private void DisposeTextureResources()
    {
        Logger.Trace("Disposing texture resources.");
        _passBuffer.Dispose();
        _passTexture.Dispose();
        _gBuffer.Dispose();
        _metallicRoughnessTexture.Dispose();
        _normalTexture.Dispose();
        _positionTexture.Dispose();
        _albedoTexture.Dispose();
        _depthTexture.Dispose();

        if (_debugTextures != null)
        {
            foreach ((_, Texture texture) in _debugTextures)
                texture.Dispose();

            _debugTextures = null;
        }
    }

    public void Dispose()
    {
        _compositeSet.Dispose();
        _compositePipeline.Dispose();
        _passInputSet.Dispose();
        _passPipeline.Dispose();
        _drawInfoSet.Dispose();
        _drawInfoBuffer.Dispose();
        _cameraInfoSet.Dispose();
        _cameraInfoBuffer.Dispose();
        
        MaterialInfoLayout.Dispose();
        DrawInfoLayout.Dispose();
        CameraInfoLayout.Dispose();
        GBufferPixelModule.Dispose();
        GBufferVertexModule.Dispose();
        DisposeTextureResources();
    }
}
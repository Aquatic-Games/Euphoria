using System;
using System.Collections.Generic;
using System.Numerics;
using Euphoria.Render.Renderers.Structs;
using grabs.Graphics;
using u4.Math;
using Buffer = grabs.Graphics.Buffer;

namespace Euphoria.Render.Renderers;

public class Renderer3D : IDisposable
{
    private readonly Device _device;
    private Size<int> _size;
    
    private readonly List<TransformedRenderable> _opaques;
    
    private readonly GrabsTexture _depthTexture;
    
    private readonly GrabsTexture _albedoTexture;
    private readonly GrabsTexture _positionTexture;
    private readonly Framebuffer _gBuffer;

    private readonly GrabsTexture _passTexture;
    private readonly Framebuffer _passBuffer;
    private readonly Texture _passDrawTexture; // TODO: This is temporary - the renderer should not use the texture batcher to draw, as it will be faster to draw directly using a shader.
    
    private readonly DescriptorLayout _materialInfoLayout;
    
    private readonly Pipeline _defaultPipeline;
    
    private readonly Pipeline _passPipeline;
    
    private Buffer _cameraInfoBuffer;
    private DescriptorSet _cameraInfoSet;
    
    private Buffer _drawInfoBuffer;
    private DescriptorSet _drawInfoSet;

    private DescriptorSet _passInputSet;
    
    public CameraInfo Camera;

    public Renderer3D(Device device, Size<int> size, DescriptorLayout batcherLayout)
    {
        _device = device;
        _size = size;
        _opaques = new List<TransformedRenderable>();
        
        TextureDescription textureDesc = TextureDescription.Texture2D((uint) size.Width, (uint) size.Height, 1,
            Format.D32_Float, TextureUsage.None);
        
        // TODO GRABS doesn't allow a TextureUsage of Framebuffer if creating a depth texture, D3D11 crashes - perhaps this should be fixed?
        _depthTexture = device.CreateTexture(textureDesc);
        
        textureDesc.Format = Format.R32G32B32A32_Float;
        textureDesc.Usage = TextureUsage.Framebuffer | TextureUsage.ShaderResource;

        _albedoTexture = device.CreateTexture(textureDesc);
        _positionTexture = device.CreateTexture(textureDesc);
        _gBuffer = device.CreateFramebuffer([_albedoTexture, _positionTexture], _depthTexture);

        using ShaderModule gBufferVertex = device.CreateShaderModule(ShaderStage.Vertex,
            ShaderLoader.LoadSpirvShader("Deferred/GBuffer", ShaderStage.Vertex), "Vertex");
        using ShaderModule gBufferPixel = device.CreateShaderModule(ShaderStage.Pixel,
            ShaderLoader.LoadSpirvShader("Deferred/GBuffer", ShaderStage.Pixel), "Pixel");

        using DescriptorLayout cameraInfoLayout = device.CreateDescriptorLayout(
            new DescriptorLayoutDescription(new DescriptorBindingDescription(0, DescriptorType.ConstantBuffer,
                ShaderStage.Vertex)));
        using DescriptorLayout drawInfoLayout = device.CreateDescriptorLayout(
            new DescriptorLayoutDescription(new DescriptorBindingDescription(0, DescriptorType.ConstantBuffer,
                ShaderStage.Vertex)));
        _materialInfoLayout = device.CreateDescriptorLayout(
            new DescriptorLayoutDescription(new DescriptorBindingDescription(0, DescriptorType.Texture,
                ShaderStage.Pixel)));

        PipelineDescription defaultPipelineDesc = new PipelineDescription(gBufferVertex, gBufferPixel,
            [
                new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex),
                new InputLayoutDescription(Format.R32G32_Float, 12, 0, InputType.PerVertex),
                new InputLayoutDescription(Format.R32G32B32A32_Float, 20, 0, InputType.PerVertex),
                new InputLayoutDescription(Format.R32G32B32_Float, 36, 0, InputType.PerVertex)
            ], DepthStencilDescription.DepthLessEqual, RasterizerDescription.CullNone,
            [cameraInfoLayout, drawInfoLayout, _materialInfoLayout]);

        _defaultPipeline = device.CreatePipeline(defaultPipelineDesc);

        _cameraInfoBuffer = device.CreateBuffer(BufferType.Constant, Camera, true);
        _cameraInfoSet =
            device.CreateDescriptorSet(cameraInfoLayout, new DescriptorSetDescription(buffer: _cameraInfoBuffer));
        
        _drawInfoBuffer = device.CreateBuffer(BufferType.Constant, Matrix4x4.Identity, true);
        _drawInfoSet =
            device.CreateDescriptorSet(drawInfoLayout, new DescriptorSetDescription(buffer: _drawInfoBuffer));
        
        _passTexture = device.CreateTexture(textureDesc);
        _passBuffer = device.CreateFramebuffer([_passTexture], _depthTexture);

        _passDrawTexture = new Texture(_passTexture,
            device.CreateDescriptorSet(batcherLayout, new DescriptorSetDescription(texture: _passTexture)), _size);

        using ShaderModule passVertex = device.CreateShaderModule(ShaderStage.Vertex,
            ShaderLoader.LoadSpirvShader("Deferred/LightingPass", ShaderStage.Vertex), "Vertex");
        using ShaderModule passPixel = device.CreateShaderModule(ShaderStage.Pixel,
            ShaderLoader.LoadSpirvShader("Deferred/LightingPass", ShaderStage.Pixel), "Pixel");

        using DescriptorLayout passInputLayout = device.CreateDescriptorLayout(
            new DescriptorLayoutDescription(
                new DescriptorBindingDescription(0, DescriptorType.Texture, ShaderStage.Pixel), // Albedo
                new DescriptorBindingDescription(1, DescriptorType.Texture, ShaderStage.Pixel) // Position
                )
            );

        PipelineDescription passPipelineDesc = new PipelineDescription(passVertex, passPixel, null,
            DepthStencilDescription.Disabled, RasterizerDescription.CullNone, [passInputLayout]);

        _passPipeline = device.CreatePipeline(passPipelineDesc);

        _passInputSet = device.CreateDescriptorSet(passInputLayout,
            new DescriptorSetDescription(texture: _albedoTexture),
            new DescriptorSetDescription(texture: _positionTexture));
    }

    public void Draw(Renderable renderable, in Matrix4x4 world)
    {
        _opaques.Add(new TransformedRenderable(renderable, world));
    }

    internal void Render(CommandList cl, Framebuffer swapchainBuffer, TextureBatcher batcher)
    {
        cl.UpdateBuffer(_cameraInfoBuffer, 0, Camera);
        cl.SetDescriptorSet(0, _cameraInfoSet);
        
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

        RenderPassDescription lightingPassDesc = new RenderPassDescription(_passBuffer, new Vector4(1.0f, 0.5f, 0.25f, 1.0f));
        cl.BeginRenderPass(lightingPassDesc);
        
        cl.SetDescriptorSet(0, _passInputSet);
        cl.SetPipeline(_passPipeline);
        
        cl.Draw(6);
        
        cl.EndRenderPass();

        RenderPassDescription compositePassDesc = new RenderPassDescription(swapchainBuffer, new Vector4(0, 0, 0, 1));
        cl.BeginRenderPass(compositePassDesc);
        
        batcher.Draw(_passDrawTexture, Vector2.Zero, Color.White);
        batcher.DispatchDrawQueue(cl, _size);
        
        cl.EndRenderPass();
        
        // TODO: Don't clear here, have a "NewFrame" method or something. Would allow support for multiple cameras.
        _opaques.Clear();
    }
    
    public Renderable CreateRenderable(Mesh mesh, Material material)
    {
        Buffer vertexBuffer = _device.CreateBuffer(BufferType.Vertex, mesh.Vertices);
        Buffer indexBuffer = _device.CreateBuffer(BufferType.Index, mesh.Indices);

        return new Renderable(vertexBuffer, indexBuffer, (uint) mesh.Indices.Length, material);
    }

    public Material CreateMaterial(in MaterialDescription description)
    {
        DescriptorSet matDescriptor = _device.CreateDescriptorSet(_materialInfoLayout,
            new DescriptorSetDescription(texture: description.Albedo.GTexture));

        return new Material(description.Albedo, _defaultPipeline, matDescriptor);
    }

    public void Dispose()
    {
        _defaultPipeline.Dispose();
        
        _materialInfoLayout.Dispose();
        
        _gBuffer.Dispose();
        _depthTexture.Dispose();
        _positionTexture.Dispose();
        _albedoTexture.Dispose();
    }
}
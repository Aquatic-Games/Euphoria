using System;
using System.Collections.Generic;
using Euphoria.Render.Renderers.Structs;
using grabs.Graphics;
using u4.Math;
using Buffer = grabs.Graphics.Buffer;

namespace Euphoria.Render.Renderers;

public class Renderer3D : IDisposable
{
    private readonly Device _device;
    private readonly List<TransformedRenderable> _opaques;
    
    private readonly Framebuffer _gBuffer;
    private readonly GrabsTexture _albedoTexture;
    private readonly GrabsTexture _positionTexture;
    
    private readonly GrabsTexture _depthTexture;

    private readonly DescriptorLayout _cameraInfoLayout;
    private readonly DescriptorLayout _drawInfoLayout;
    private readonly DescriptorLayout _materialInfoLayout;
    
    private readonly Pipeline _defaultPipeline;
    
    public CameraInfo Camera;

    public Renderer3D(Device device, Size<int> size, ShaderLoader loader)
    {
        _device = device;
        _opaques = new List<TransformedRenderable>();
        
        TextureDescription textureDesc = TextureDescription.Texture2D((uint) size.Width, (uint) size.Height, 1,
            Format.R32G32B32A32_Float, TextureUsage.Framebuffer | TextureUsage.ShaderResource);

        _albedoTexture = device.CreateTexture(textureDesc);
        _positionTexture = device.CreateTexture(textureDesc);

        textureDesc.Format = Format.D32_Float;
        textureDesc.Usage = TextureUsage.Framebuffer;
        _depthTexture = device.CreateTexture(textureDesc);

        _gBuffer = device.CreateFramebuffer([_albedoTexture, _positionTexture], _depthTexture);

        ShaderModule gBufferVertex = device.CreateShaderModule(ShaderStage.Vertex,
            loader.LoadSpirvShader("GBuffer", ShaderStage.Vertex), "Vertex");
        ShaderModule gBufferPixel = device.CreateShaderModule(ShaderStage.Pixel,
            loader.LoadSpirvShader("GBuffer", ShaderStage.Pixel), "Pixel");

        _cameraInfoLayout = device.CreateDescriptorLayout(
            new DescriptorLayoutDescription(new DescriptorBindingDescription(0, DescriptorType.ConstantBuffer,
                ShaderStage.Vertex)));
        _drawInfoLayout = device.CreateDescriptorLayout(
            new DescriptorLayoutDescription(new DescriptorBindingDescription(0, DescriptorType.ConstantBuffer,
                ShaderStage.Vertex)));
        _materialInfoLayout = device.CreateDescriptorLayout(
            new DescriptorLayoutDescription(new DescriptorBindingDescription(0, DescriptorType.Texture,
                ShaderStage.Pixel)));

        PipelineDescription defaultPipelineDesc = new PipelineDescription(gBufferVertex, gBufferPixel,
            [
                new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex),
                new InputLayoutDescription(Format.R32G32_Float, 12, 0, InputType.PerVertex)
            ], DepthStencilDescription.DepthLessEqual, RasterizerDescription.CullCounterClockwise,
            [_cameraInfoLayout, _drawInfoLayout, _materialInfoLayout]);

        _defaultPipeline = device.CreatePipeline(defaultPipelineDesc);
    }
    
    public Renderable CreateRenderable(Mesh mesh)
    {
        Buffer vertexBuffer = _device.CreateBuffer(BufferType.Vertex, mesh.Vertices);
        Buffer indexBuffer = _device.CreateBuffer(BufferType.Index, mesh.Indices);

        return new Renderable(vertexBuffer, indexBuffer, (uint) mesh.Indices.Length);
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
        _drawInfoLayout.Dispose();
        _cameraInfoLayout.Dispose();
        
        _gBuffer.Dispose();
        _depthTexture.Dispose();
        _positionTexture.Dispose();
        _albedoTexture.Dispose();
    }
}
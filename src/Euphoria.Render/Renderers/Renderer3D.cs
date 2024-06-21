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
    private readonly List<TransformedRenderable> _opaques;
    
    private readonly Framebuffer _gBuffer;
    private readonly GrabsTexture _albedoTexture;
    private readonly GrabsTexture _positionTexture;
    
    private readonly GrabsTexture _depthTexture;
    
    private readonly DescriptorLayout _materialInfoLayout;
    
    private Buffer _cameraInfoBuffer;
    private DescriptorSet _cameraInfoSet;
    
    private Buffer _drawInfoBuffer;
    private DescriptorSet _drawInfoSet;
    
    private readonly Pipeline _defaultPipeline;
    
    public CameraInfo Camera;

    public Renderer3D(Device device, Size<int> size)
    {
        _device = device;
        _opaques = new List<TransformedRenderable>();
        
        TextureDescription textureDesc = TextureDescription.Texture2D((uint) size.Width, (uint) size.Height, 1,
            Format.R32G32B32A32_Float, TextureUsage.Framebuffer | TextureUsage.ShaderResource);

        _albedoTexture = device.CreateTexture(textureDesc);
        _positionTexture = device.CreateTexture(textureDesc);

        // TODO GRABS doesn't allow a TextureUsage of Framebuffer if creating a depth texture, D3D11 crashes - perhaps this should be fixed?
        textureDesc.Format = Format.D32_Float;
        textureDesc.Usage = TextureUsage.None;
        _depthTexture = device.CreateTexture(textureDesc);

        _gBuffer = device.CreateFramebuffer([_albedoTexture, _positionTexture], _depthTexture);

        ShaderModule gBufferVertex = device.CreateShaderModule(ShaderStage.Vertex,
            ShaderLoader.LoadSpirvShader("GBuffer", ShaderStage.Vertex), "Vertex");
        ShaderModule gBufferPixel = device.CreateShaderModule(ShaderStage.Pixel,
            ShaderLoader.LoadSpirvShader("GBuffer", ShaderStage.Pixel), "Pixel");

        DescriptorLayout cameraInfoLayout = device.CreateDescriptorLayout(
            new DescriptorLayoutDescription(new DescriptorBindingDescription(0, DescriptorType.ConstantBuffer,
                ShaderStage.Vertex)));
        DescriptorLayout drawInfoLayout = device.CreateDescriptorLayout(
            new DescriptorLayoutDescription(new DescriptorBindingDescription(0, DescriptorType.ConstantBuffer,
                ShaderStage.Vertex)));
        _materialInfoLayout = device.CreateDescriptorLayout(
            new DescriptorLayoutDescription(new DescriptorBindingDescription(0, DescriptorType.Texture,
                ShaderStage.Pixel)));

        PipelineDescription defaultPipelineDesc = new PipelineDescription(gBufferVertex, gBufferPixel,
            [
                new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex),
                new InputLayoutDescription(Format.R32G32_Float, 12, 0, InputType.PerVertex)
            ], DepthStencilDescription.DepthLessEqual, RasterizerDescription.CullNone,
            [cameraInfoLayout, drawInfoLayout, _materialInfoLayout]);

        _defaultPipeline = device.CreatePipeline(defaultPipelineDesc);

        _cameraInfoBuffer = device.CreateBuffer(BufferType.Constant, Camera, true);
        _cameraInfoSet =
            device.CreateDescriptorSet(cameraInfoLayout, new DescriptorSetDescription(buffer: _cameraInfoBuffer));
        
        _drawInfoBuffer = device.CreateBuffer(BufferType.Constant, Matrix4x4.Identity, true);
        _drawInfoSet =
            device.CreateDescriptorSet(drawInfoLayout, new DescriptorSetDescription(buffer: _drawInfoBuffer));
        
        drawInfoLayout.Dispose();
        cameraInfoLayout.Dispose();
    }

    public void Draw(Renderable renderable, in Matrix4x4 world)
    {
        _opaques.Add(new TransformedRenderable(renderable, world));
    }

    internal void Render(CommandList cl)
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
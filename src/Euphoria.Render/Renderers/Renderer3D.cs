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
    
    private readonly Framebuffer _gBuffer;
    private readonly GrabsTexture _albedoTexture;
    private readonly GrabsTexture _positionTexture;
    
    private readonly GrabsTexture _depthTexture;

    private readonly List<TransformedRenderable> _opaques;
    
    public CameraInfo Camera;

    public Renderer3D(Device device, Size<int> size, ShaderLoader loader)
    {
        _device = device;
        
        TextureDescription textureDesc = TextureDescription.Texture2D((uint) size.Width, (uint) size.Height, 1,
            Format.R32G32B32A32_Float, TextureUsage.Framebuffer | TextureUsage.ShaderResource);

        _albedoTexture = device.CreateTexture(textureDesc);
        _positionTexture = device.CreateTexture(textureDesc);

        textureDesc.Format = Format.D32_Float;
        textureDesc.Usage = TextureUsage.Framebuffer;
        _depthTexture = device.CreateTexture(textureDesc);

        _gBuffer = device.CreateFramebuffer([_albedoTexture, _positionTexture], _depthTexture);

        _opaques = new List<TransformedRenderable>();

        ShaderModule gBufferVertex = device.CreateShaderModule(ShaderStage.Vertex,
            loader.LoadSpirvShader("GBuffer", ShaderStage.Vertex), "Vertex");
        ShaderModule gBufferPixel = device.CreateShaderModule(ShaderStage.Pixel,
            loader.LoadSpirvShader("GBuffer", ShaderStage.Pixel), "Pixel");
        
        
    }
    
    public Renderable CreateRenderable(Mesh mesh)
    {
        Buffer vertexBuffer = _device.CreateBuffer(BufferType.Vertex, mesh.Vertices);
        Buffer indexBuffer = _device.CreateBuffer(BufferType.Index, mesh.Indices);

        return new Renderable(vertexBuffer, indexBuffer, (uint) mesh.Indices.Length);
    }

    public void Dispose()
    {
        _gBuffer.Dispose();
        _depthTexture.Dispose();
        _positionTexture.Dispose();
        _albedoTexture.Dispose();
    }
}
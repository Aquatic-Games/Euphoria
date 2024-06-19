using System;
using System.Collections.Generic;
using Euphoria.Render.Renderers.Structs;
using grabs.Graphics;
using u4.Math;

namespace Euphoria.Render.Renderers;

public class Renderer3D : IDisposable
{
    private readonly Framebuffer _gBuffer;
    private readonly GrabsTexture _albedoTexture;
    private readonly GrabsTexture _positionTexture;
    
    private readonly GrabsTexture _depthTexture;

    private readonly List<TransformedRenderable> _opaques;
    
    public CameraInfo Camera;

    public Renderer3D(Device device, Size<int> size, ShaderLoader loader)
    {
        TextureDescription textureDesc = TextureDescription.Texture2D((uint) size.Width, (uint) size.Height, 1,
            Format.R32G32B32A32_Float, TextureUsage.Framebuffer | TextureUsage.ShaderResource);

        _albedoTexture = device.CreateTexture(textureDesc);
        _positionTexture = device.CreateTexture(textureDesc);

        textureDesc.Format = Format.D32_Float;
        textureDesc.Usage = TextureUsage.Framebuffer;
        _depthTexture = device.CreateTexture(textureDesc);

        _gBuffer = device.CreateFramebuffer([_albedoTexture, _positionTexture], _depthTexture);

        _opaques = new List<TransformedRenderable>();
    }

    public void Dispose()
    {
        _gBuffer.Dispose();
        _depthTexture.Dispose();
        _positionTexture.Dispose();
        _albedoTexture.Dispose();
    }
}
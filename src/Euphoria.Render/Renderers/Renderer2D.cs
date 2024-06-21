using System;
using System.Numerics;
using Euphoria.Render.Renderers.Structs;
using grabs.Graphics;
using u4.Math;

namespace Euphoria.Render.Renderers;

public sealed class Renderer2D : IDisposable
{
    private Size<int> _size;
    private TextureBatcher _batcher;
    
    internal readonly Framebuffer Framebuffer;
    internal readonly GrabsTexture ColorTexture;

    public CameraInfo Camera;

    public Renderer2D(Device device, Size<int> size)
    {
        _size = size;
        _batcher = new TextureBatcher(device);
        
        ColorTexture = device.CreateTexture(TextureDescription.Texture2D((uint) size.Width, (uint) size.Height, 1,
            Format.R8G8B8A8_UNorm, TextureUsage.Framebuffer | TextureUsage.ShaderResource));

        Framebuffer = device.CreateFramebuffer(new ReadOnlySpan<GrabsTexture>(ref ColorTexture));

        Camera = new CameraInfo(Matrix4x4.CreateOrthographic(_size.Width, _size.Height, -1, 1), Matrix4x4.Identity);
    }

    public void DrawSprite(Sprite sprite)
    {
        /*Size<int> size = sprite.Texture.Size;
        Vector2 position = new Vector2(sprite.Position.X, sprite.Position.Y) - new Vector2(size.Width / 2, size.Height / 2);
        
        Matrix4x4 rotMatrix = 
     
        // Currently a TextureBatcher assumes a top left origin point and adjusts the texture coordinates for such.
        // However the default behaviour of a Renderer2D is to use a regular orthographic matrix, where the texture
        // coordinates are NOT flipped. So we have to rotate the vertices instead.
        // TODO: This behaviour should probably not be the case, instead the TextureBatcher should be made more versatile to support both behaviour.
        Vector2 topLeft = position + new Vector2(0, size.Height);
        Vector2 topRight = position + new Vector2(size.Width, size.Height);
        Vector2 bottomLeft = position;
        Vector2 bottomRight = position + new Vector2(size.Width, 0);
        
        _batcher.Draw(sprite.Texture, topLeft, topRight, bottomLeft, bottomRight, Color.White, sprite.Position.Z);*/

        Matrix3x2 world = Matrix3x2.CreateScale(1, -1) *
                          Matrix3x2.CreateTranslation(-sprite.Texture.Size.Width / 2, sprite.Texture.Size.Height / 2) *
                          sprite.World;
        
        _batcher.Draw(sprite.Texture, world, Color.White, sprite.ZIndex);
    }

    internal void DispatchRender(Device device, CommandList cl, Framebuffer drawBuffer)
    {
        cl.BeginRenderPass(new RenderPassDescription(drawBuffer, new Vector4(0, 0, 0, 1)));
        _batcher.DispatchDrawQueue(cl, _size, Camera.Projection * Camera.View, TextureBatcher.SortMode.LowestFirst);
        cl.EndRenderPass();
    }
    
    public void Dispose()
    {
        Framebuffer.Dispose();
        ColorTexture.Dispose();
    }
}
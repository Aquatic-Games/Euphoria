using System;
using Euphoria.Math;
using grabs.Graphics;

namespace Euphoria.Render;

public class Texture : IDisposable
{
    private Graphics _graphics;
    
    internal readonly GrabsTexture GTexture;
    internal readonly DescriptorSet DescriptorSet;

    public readonly ulong Id;
    
    public readonly Size<int> Size;

    internal Texture(Graphics graphics, GrabsTexture gTexture, DescriptorSet descriptorSet, ulong id, Size<int> size)
    {
        _graphics = graphics;
        GTexture = gTexture;
        DescriptorSet = descriptorSet;
        Id = id;
        Size = size;
    }

    public void Dispose()
    {
        DescriptorSet.Dispose();
        GTexture.Dispose();
        
        _graphics.Textures.RemoveItem(Id);
    }
}
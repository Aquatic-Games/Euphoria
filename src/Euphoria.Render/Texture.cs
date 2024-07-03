using System;
using Euphoria.Math;
using grabs.Graphics;

namespace Euphoria.Render;

public class Texture : IDisposable
{
    internal readonly GrabsTexture GTexture;
    internal readonly DescriptorSet DescriptorSet;

    public readonly ulong Id;
    
    public readonly Size<int> Size;

    internal Texture(GrabsTexture gTexture, DescriptorSet descriptorSet, ulong id, Size<int> size)
    {
        GTexture = gTexture;
        DescriptorSet = descriptorSet;
        Id = id;
        Size = size;
    }

    public void Dispose()
    {
        DescriptorSet.Dispose();
        GTexture.Dispose();
        
        Graphics.Textures.RemoveItem(Id);
    }
}
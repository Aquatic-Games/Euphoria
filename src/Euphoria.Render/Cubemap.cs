using System;
using Euphoria.Math;
using grabs.Graphics;

namespace Euphoria.Render;

public class Cubemap : IDisposable
{
    internal readonly GrabsTexture GTexture;
    internal readonly DescriptorSet DescriptorSet;
    
    public readonly Size<int> Size;

    public Cubemap(GrabsTexture gTexture, DescriptorSet descriptorSet, Size<int> size)
    {
        GTexture = gTexture;
        DescriptorSet = descriptorSet;
        Size = size;
    }

    public void Dispose()
    {
        DescriptorSet.Dispose();
        GTexture.Dispose();
    }
}
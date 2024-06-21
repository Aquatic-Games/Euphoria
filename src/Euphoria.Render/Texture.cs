using System;
using grabs.Graphics;
using u4.Math;

namespace Euphoria.Render;

public class Texture : IDisposable
{
    internal readonly GrabsTexture GTexture;
    internal readonly DescriptorSet DescriptorSet;

    public readonly Size<int> Size;

    internal Texture(GrabsTexture gTexture, DescriptorSet descriptorSet, Size<int> size)
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
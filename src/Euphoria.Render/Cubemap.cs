using System;
using Euphoria.Math;
using grabs.Graphics;

namespace Euphoria.Render;

public class Cubemap : IDisposable
{
    internal readonly GrabsTexture GTexture;
    internal readonly Sampler Sampler;
    internal readonly DescriptorSet DescriptorSet;
    
    public readonly Size<int> Size;

    public Cubemap(Bitmap right, Bitmap left, Bitmap top, Bitmap bottom, Bitmap front, Bitmap back, SamplerDescription? sampler = null)
    {
        Size = right.Size;

        Device device = Graphics.Device;

        TextureDescription desc = TextureDescription.Cubemap((uint) Size.Width, (uint) Size.Height, 0, right.Format,
            TextureUsage.ShaderResource | TextureUsage.GenerateMips);

        GTexture = device.CreateTexture(desc, [right.Data, left.Data, top.Data, bottom.Data, front.Data, back.Data]);

        Sampler = device.CreateSampler(sampler ?? SamplerDescription.LinearClamp);

        DescriptorSet = device.CreateDescriptorSet(Graphics.TextureDescriptorLayout,
            new DescriptorSetDescription(texture: GTexture, sampler: Sampler));
        
        Graphics.TexturesQueuedForMipGeneration.Add(GTexture);
    }

    public void Dispose()
    {
        DescriptorSet.Dispose();
        GTexture.Dispose();
    }
}
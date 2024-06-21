using System;
using grabs.Graphics;

namespace Euphoria.Render;

public sealed class Material : IDisposable
{
    private Texture _albedo;

    internal readonly Pipeline Pipeline;
    internal readonly DescriptorSet MatDescriptor;

    public Texture Albedo => _albedo;

    public Material(Texture albedo, Pipeline pipeline, DescriptorSet matDescriptor)
    {
        _albedo = albedo;

        Pipeline = pipeline;
        MatDescriptor = matDescriptor;
    }

    public void Dispose()
    {
        MatDescriptor.Dispose();
    }
}
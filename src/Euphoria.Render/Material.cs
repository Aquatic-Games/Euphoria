using System;
using Euphoria.Render.Renderers;
using grabs.Graphics;

namespace Euphoria.Render;

public sealed class Material : IDisposable
{
    private Texture _albedo;
    private Texture _normal;

    internal readonly Pipeline Pipeline;
    internal readonly DescriptorSet MatDescriptor;

    public Texture Albedo => _albedo;

    public Texture Normal => _normal;

    public Material(in MaterialDescription description)
    {
        _albedo = description.Albedo;
        _normal = description.Normal;

        Device device = Graphics.Device;
        Renderer3D renderer = Graphics.Renderer3D;
        
        PipelineDescription pipelineDesc = new PipelineDescription(renderer.GBufferVertexModule,
            renderer.GBufferPixelModule, renderer.GBufferInputLayout, description.Depth, description.Rasterizer,
            BlendDescription.Disabled,
            [renderer.CameraInfoLayout, renderer.DrawInfoLayout, renderer.MaterialInfoLayout],
            description.PrimitiveType);

        Pipeline = device.CreatePipeline(pipelineDesc);

        MatDescriptor = device.CreateDescriptorSet(renderer.MaterialInfoLayout,
            new DescriptorSetDescription(texture: _albedo.GTexture),
            new DescriptorSetDescription(texture: _normal.GTexture));
    }

    public void Dispose()
    {
        MatDescriptor.Dispose();
        Pipeline.Dispose();
    }
}
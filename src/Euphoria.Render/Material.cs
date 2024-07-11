using System;
using Euphoria.Render.Renderers;
using grabs.Graphics;

namespace Euphoria.Render;

public sealed class Material : IDisposable
{
    private Texture _albedo;
    private Texture _normal;
    private Texture _metallic;
    private Texture _roughness;
    private Texture _occlusion;
    
    internal readonly Pipeline Pipeline;
    internal readonly DescriptorSet MatDescriptor;

    public Texture Albedo => _albedo;

    public Texture Normal => _normal;

    public Texture Metallic => _metallic;

    public Texture Roughness => _roughness;

    public Texture Occlusion => _occlusion;

    public Material(in MaterialDescription description)
    {
        _albedo = description.Albedo;
        _normal = description.Normal;
        _metallic = description.Metallic;
        _roughness = description.Roughness;
        _occlusion = description.Occlusion;

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
            new DescriptorSetDescription(texture: _normal.GTexture),
            new DescriptorSetDescription(texture: _metallic.GTexture),
            new DescriptorSetDescription(texture: _roughness.GTexture),
            new DescriptorSetDescription(texture: _occlusion.GTexture));
    }

    public void Dispose()
    {
        MatDescriptor.Dispose();
        Pipeline.Dispose();
    }
}
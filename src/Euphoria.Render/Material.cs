using System;
using System.Collections.Generic;
using Euphoria.Core;
using Euphoria.Math;
using Euphoria.Render.Renderers;
using Euphoria.Render.Renderers.Structs;
using Euphoria.Render.Structs.Internal;
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

    public readonly ulong Id;

    public Color AlbedoColor;

    public float MetallicColor;

    public float RoughnessColor;

    public Texture Albedo => _albedo;

    public Texture Normal => _normal;

    public Texture Metallic => _metallic;

    public Texture Roughness => _roughness;

    public Texture Occlusion => _occlusion;

    public MaterialDescription Description { get; }

    internal MaterialInfo MaterialInfo => new MaterialInfo(AlbedoColor, MetallicColor, RoughnessColor);

    public Material(in MaterialDescription description)
    {
        _albedo = description.Albedo;
        _normal = description.Normal;
        _metallic = description.Metallic;
        _roughness = description.Roughness;
        _occlusion = description.Occlusion;

        AlbedoColor = description.AlbedoColor;
        MetallicColor = description.MetallicColor;
        RoughnessColor = description.RoughnessColor;

        Description = description;

        Id = _loadedMaterials.AddItem(this);
        Logger.Trace($"Creating material {Id}.");
        
        Device device = Graphics.Device;
        Renderer3D renderer = Graphics.Renderer3D;
        
        PipelineDescription pipelineDesc = new PipelineDescription(renderer.GBufferVertexModule,
            renderer.GBufferPixelModule, renderer.GBufferInputLayout, description.Depth, description.Rasterizer,
            BlendDescription.Disabled,
            [renderer.CameraInfoLayout, renderer.DrawInfoLayout, renderer.MaterialInfoLayout],
            description.PrimitiveType);

        Pipeline = device.CreatePipeline(pipelineDesc);

        MatDescriptor = device.CreateDescriptorSet(renderer.MaterialInfoLayout,
            new DescriptorSetDescription(texture: _albedo.GTexture, sampler: _albedo.Sampler),
            new DescriptorSetDescription(texture: _normal.GTexture, sampler: _normal.Sampler),
            new DescriptorSetDescription(texture: _metallic.GTexture, sampler: _metallic.Sampler),
            new DescriptorSetDescription(texture: _roughness.GTexture, sampler: _roughness.Sampler),
            new DescriptorSetDescription(texture: _occlusion.GTexture, sampler: _occlusion.Sampler));
    }

    public void Dispose()
    {
        MatDescriptor.Dispose();
        Pipeline.Dispose();
    }

    private static ItemIdCollection<Material> _loadedMaterials;
    private static Dictionary<string, ulong> _namedMaterials;

    static Material()
    {
        _loadedMaterials = new ItemIdCollection<Material>();
        _namedMaterials = new Dictionary<string, ulong>();
        
        Logger.Trace("Creating default materials.");
        Default = new Material(new MaterialDescription(Texture.White));
    }

    public static void StoreMaterial(string name, Material material)
    {
        Logger.Trace($"Assigning name \"{name}\" to material {material.Id}");
        _namedMaterials[name] = material.Id;
    }

    public static Material GetMaterial(ulong id)
        => _loadedMaterials[id];

    public static Material GetMaterial(string name)
        => _loadedMaterials[_namedMaterials[name]];

    public IEnumerable<Material> GetAllMaterials()
    {
        foreach ((_, Material material) in _loadedMaterials.Items)
            yield return material;
    }

    public static void DisposeAllMaterials()
    {
        Logger.Debug("Disposing all materials.");
        
        foreach ((_, Material material) in _loadedMaterials.Items)
            material.Dispose();
        
        _loadedMaterials.Items.Clear();
        _namedMaterials.Clear();
    }

    public static readonly Material Default;
}
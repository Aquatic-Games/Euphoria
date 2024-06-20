using System;
using grabs.Graphics;

namespace Euphoria.Render;

public sealed class Material : IDisposable
{
    private Graphics _graphics;
    private Texture _albedo;

    internal DescriptorSet DescriptorSet;
    internal Pipeline Pipeline;

    public Texture Albedo
    {
        get => _albedo;
        set
        {
            _albedo = value;
            UpdateDescriptor();
        }
    }
    
    internal Material(Graphics graphics, Texture albedo)
    {
        _graphics = graphics;
        _albedo = albedo;

        Device device = graphics.Device;
        DescriptorSet = device.CreateDescriptorSet()
    }

    private void UpdateDescriptor()
    {
        
    }
}
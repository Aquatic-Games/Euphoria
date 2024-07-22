using System;
using System.Collections.Generic;
using Euphoria.Core;
using Euphoria.Math;
using grabs.Graphics;

namespace Euphoria.Render;

public class Texture : IDisposable
{
    private bool _ownsTexture;
    
    internal readonly GrabsTexture GTexture;
    internal readonly Sampler Sampler;
    internal readonly DescriptorSet DescriptorSet;

    public readonly ulong Id;
    
    public readonly Size<int> Size;

    public Format Format => GTexture.Description.Format;

    public Texture(string path, SamplerDescription? sampler = null) : this(new Bitmap(path), sampler) { }

    public Texture(Bitmap bitmap, SamplerDescription? sampler = null) : this(bitmap.Data, bitmap.Size, bitmap.Format, sampler) { }

    public Texture(byte[] data, Size<int> size, Format format = Format.R8G8B8A8_UNorm, SamplerDescription? sampler = null)
    {
        _ownsTexture = true;
        Size = size;
        
        Device device = Graphics.Device;

        TextureDescription desc = TextureDescription.Texture2D((uint) size.Width, (uint) size.Height, 0, format,
            TextureUsage.ShaderResource | TextureUsage.GenerateMips);

        if (data == null)
            GTexture = device.CreateTexture(desc);
        else
            GTexture = device.CreateTexture(desc, data);
        Graphics.TexturesQueuedForMipGeneration.Add(GTexture);

        Sampler = device.CreateSampler(sampler ?? SamplerDescription.LinearClamp);

        DescriptorSet = device.CreateDescriptorSet(Graphics.TextureDescriptorLayout,
            new DescriptorSetDescription(texture: GTexture, sampler: Sampler));

        Id = _loadedTextures.AddItem(this);
    }

    internal Texture(GrabsTexture texture, Sampler sampler, Size<int> size, bool ownsTexture = true)
    {
        _ownsTexture = ownsTexture;
        GTexture = texture;
        Sampler = sampler;
        Size = size;
        
        DescriptorSet = Graphics.Device.CreateDescriptorSet(Graphics.TextureDescriptorLayout,
            new DescriptorSetDescription(texture: GTexture));

        Id = _loadedTextures.AddItem(this);
    }

    public void Update(int x, int y, int width, int height, byte[] data)
    {
        Graphics.Device.UpdateTexture(GTexture, x, y, (uint) width, (uint) height, 0, data);
        Graphics.TexturesQueuedForMipGeneration.Add(GTexture);
    }

    public void Dispose()
    {
        DescriptorSet.Dispose();

        if (_ownsTexture)
        {
            Sampler.Dispose();
            GTexture.Dispose();
        }

        _loadedTextures.RemoveItem(Id);
    }
    
    private static ItemIdCollection<Texture> _loadedTextures;
    private static Dictionary<string, ulong> _namedTextures;

    static Texture()
    {
        _loadedTextures = new ItemIdCollection<Texture>();
        _namedTextures = new Dictionary<string, ulong>();
        
        Logger.Trace("Creating default textures.");
        White = new Texture([255, 255, 255, 255], new Size<int>(1));
        Black = new Texture([0, 0, 0, 255], new Size<int>(1));
        EmptyNormal = new Texture([128, 128, 255, 255], new Size<int>(1));
    }

    public static void StoreTexture(string name, Texture texture)
    {
        _namedTextures[name] = texture.Id;
    }

    public static Texture GetTexture(ulong id)
        => _loadedTextures[id];

    public static Texture GetTexture(string name)
        => _loadedTextures[_namedTextures[name]];

    public static IEnumerable<Texture> GetAllTextures()
    {
        foreach ((_, Texture texture) in _loadedTextures.Items)
            yield return texture;
    }

    public static void DisposeAllTextures()
    {
        foreach ((_, Texture texture) in _loadedTextures.Items)
            texture.Dispose();
    }

    public static readonly Texture White;
    
    public static readonly Texture Black;

    public static readonly Texture EmptyNormal;
}
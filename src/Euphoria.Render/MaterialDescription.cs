using Euphoria.Math;
using grabs.Graphics;

namespace Euphoria.Render;

public struct MaterialDescription
{
    public Texture Albedo;

    public Texture Normal;

    public Texture Metallic;

    public Texture Roughness;

    public Texture Occlusion;

    public Color AlbedoColor;

    public float MetallicColor;

    public float RoughnessColor;

    public RasterizerDescription Rasterizer;
    
    public DepthStencilDescription Depth;

    public PrimitiveType PrimitiveType;

    public MaterialDescription(Texture albedo, Texture normal = null, Texture metallic = null, Texture roughness = null,
        Texture occlusion = null, Color? albedoColor = null, float metallicColor = 0.0f, float roughnessColor = 1.0f,
        RasterizerDescription? rasterizer = null, DepthStencilDescription? depth = null,
        PrimitiveType primitiveType = PrimitiveType.TriangleList)
    {
        Albedo = albedo;
        Normal = normal ?? Texture.EmptyNormal;
        Metallic = metallic ?? Texture.White;
        Roughness = roughness ?? Texture.White;
        Occlusion = occlusion ?? Texture.White;
        
        AlbedoColor = albedoColor ?? Color.White;
        MetallicColor = metallicColor;
        RoughnessColor = roughnessColor;
        
        Rasterizer = rasterizer ?? RasterizerDescription.CullClockwise;
        Depth = depth ?? DepthStencilDescription.DepthLessEqual;
        PrimitiveType = primitiveType;
    }
}
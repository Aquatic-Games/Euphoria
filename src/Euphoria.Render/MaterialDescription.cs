using grabs.Graphics;

namespace Euphoria.Render;

public struct MaterialDescription
{
    public Texture Albedo;

    public Texture Normal;

    public Texture Metallic;

    public Texture Roughness;

    public Texture Occlusion;

    public RasterizerDescription Rasterizer;
    
    public DepthStencilDescription Depth;

    public PrimitiveType PrimitiveType;

    public MaterialDescription(Texture albedo)
    {
        Albedo = albedo;
        Normal = Texture.EmptyNormal;
        Metallic = Texture.Black;
        Roughness = Texture.White;
        Occlusion = Texture.White;
        
        Rasterizer = RasterizerDescription.CullClockwise;
        Depth = DepthStencilDescription.DepthLessEqual;
        PrimitiveType = PrimitiveType.TriangleList;
    }
}
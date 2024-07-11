using grabs.Graphics;

namespace Euphoria.Render;

public struct MaterialDescription
{
    public Texture Albedo;

    public Texture Normal;

    public RasterizerDescription Rasterizer;
    
    public DepthStencilDescription Depth;

    public PrimitiveType PrimitiveType;

    public MaterialDescription(Texture albedo, Texture normal = null)
    {
        Albedo = albedo;
        Normal = normal ?? Texture.EmptyNormal;
        
        Rasterizer = RasterizerDescription.CullClockwise;
        Depth = DepthStencilDescription.DepthLessEqual;
        PrimitiveType = PrimitiveType.TriangleList;
    }
}
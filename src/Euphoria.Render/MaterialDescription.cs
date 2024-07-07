using grabs.Graphics;

namespace Euphoria.Render;

public struct MaterialDescription
{
    public Texture Albedo;

    public RasterizerDescription Rasterizer;
    
    public DepthStencilDescription Depth;

    public PrimitiveType PrimitiveType;

    public MaterialDescription(Texture albedo)
    {
        Albedo = albedo;
        
        Rasterizer = RasterizerDescription.CullClockwise;
        Depth = DepthStencilDescription.DepthLessEqual;
        PrimitiveType = PrimitiveType.TriangleList;
    }
}
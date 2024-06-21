namespace Euphoria.Render;

public struct MaterialDescription
{
    public Texture Albedo;

    public MaterialDescription(Texture albedo)
    {
        Albedo = albedo;
    }
}
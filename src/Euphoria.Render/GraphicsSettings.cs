namespace Euphoria.Render;

public struct GraphicsSettings
{
    public RenderType RenderType;

    public GraphicsSettings(RenderType renderType)
    {
        RenderType = renderType;
    }

    public static GraphicsSettings Default => new GraphicsSettings(RenderType.Normal);
}
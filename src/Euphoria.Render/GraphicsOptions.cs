namespace Euphoria.Render;

public struct GraphicsOptions
{
    public RenderType RenderType;

    public GraphicsOptions(RenderType renderType)
    {
        RenderType = renderType;
    }

    public static GraphicsOptions Default => new GraphicsOptions(RenderType.Normal);
}
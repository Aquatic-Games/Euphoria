namespace Euphoria.Render;

public abstract class Graphics : IDisposable
{
    public abstract RenderAPI RenderAPI { get; }
    
    public abstract void Present();

    public abstract void Dispose();
}
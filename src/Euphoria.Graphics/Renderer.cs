namespace u4.Graphics;

public abstract class Renderer : IDisposable
{
    public abstract void Present();
    
    public virtual void Dispose() { }
}
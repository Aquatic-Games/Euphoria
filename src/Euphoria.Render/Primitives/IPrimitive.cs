namespace Euphoria.Render.Primitives;

public interface IPrimitive
{
    public Vertex[] Vertices { get; }
    
    public uint[] Indices { get; }
}
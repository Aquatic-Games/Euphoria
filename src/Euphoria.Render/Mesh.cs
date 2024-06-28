namespace Euphoria.Render;

public class Mesh
{
    public Vertex[] Vertices;
    public uint[] Indices;

    // TODO: Should a mesh have a material? Probably.
    public Mesh(Vertex[] vertices, uint[] indices)
    {
        Vertices = vertices;
        Indices = indices;
    }
}
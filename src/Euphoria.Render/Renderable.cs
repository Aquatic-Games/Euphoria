using System;
using Buffer = grabs.Graphics.Buffer;

namespace Euphoria.Render;

public sealed class Renderable : IDisposable
{
    internal readonly Buffer VertexBuffer;
    internal readonly Buffer IndexBuffer;

    internal readonly uint NumElements;

    public Material Material;

    internal Renderable(Buffer vertexBuffer, Buffer indexBuffer, uint numElements, Material material)
    {
        VertexBuffer = vertexBuffer;
        IndexBuffer = indexBuffer;
        NumElements = numElements;
        Material = material;
    }
    
    public void Dispose()
    {
        IndexBuffer.Dispose();
        VertexBuffer.Dispose();
    }
}
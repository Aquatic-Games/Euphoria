using System;
using Euphoria.Core;
using grabs.Graphics;
using Buffer = grabs.Graphics.Buffer;

namespace Euphoria.Render;

public sealed class Renderable : IDisposable
{
    private readonly Device _device;
    
    internal Buffer VertexBuffer;
    internal Buffer IndexBuffer;

    internal uint NumElements;

    public readonly UpdateFlags UpdateFlags;

    public Material Material;

    internal Renderable(Device device, Buffer vertexBuffer, Buffer indexBuffer, uint numElements, Material material, UpdateFlags updateFlags)
    {
        _device = device;
        VertexBuffer = vertexBuffer;
        IndexBuffer = indexBuffer;
        NumElements = numElements;
        Material = material;
        UpdateFlags = updateFlags;
    }

    public void Update(Mesh mesh)
    {
        bool updatable = (UpdateFlags & UpdateFlags.Updatable) == UpdateFlags.Updatable;
        bool recreateBuffers = (UpdateFlags & UpdateFlags.NoRecreateBuffers) != UpdateFlags.NoRecreateBuffers;
        bool expandBuffers = (UpdateFlags & UpdateFlags.ExpandBuffers) == UpdateFlags.ExpandBuffers;
        
        if (!updatable)
            throw new Exception("Cannot update: Renderable has not been created with \"UpdateFlags.Updatable\" flag.");

        if (mesh.Vertices.Length * Vertex.SizeInBytes > VertexBuffer.Description.SizeInBytes)
        {
            if (recreateBuffers)
            {
                uint bufferSizeInBytes = (uint) mesh.Vertices.Length * Vertex.SizeInBytes;
                Logger.Trace($"Recreating vertex buffer with size {bufferSizeInBytes}.");
                if (expandBuffers)
                {
                    bufferSizeInBytes = uint.Max(bufferSizeInBytes, VertexBuffer.Description.SizeInBytes << 1);
                    Logger.Trace($"Vertex buffer will be expanded to size {bufferSizeInBytes}.");
                }

                VertexBuffer.Dispose();
                // We don't create these buffers with any initial data, as we map them and write to them later.
                VertexBuffer =
                    _device.CreateBuffer(new BufferDescription(BufferType.Vertex, bufferSizeInBytes, updatable));
            }
            else
            {
                throw new Exception(
                    "Cannot update: Vertex data is larger than the buffer, dispose and recreate the renderable instead.");
            }
        }

        if (mesh.Indices.Length * sizeof(uint) > IndexBuffer.Description.SizeInBytes)
        {
            if (recreateBuffers)
            {
                uint bufferSizeInBytes = (uint) mesh.Indices.Length * sizeof(uint);
                Logger.Trace($"Recreating index buffer with size {bufferSizeInBytes}.");
                if (expandBuffers)
                {
                    bufferSizeInBytes = uint.Max(bufferSizeInBytes, IndexBuffer.Description.SizeInBytes << 1);
                    Logger.Trace($"Index buffer will be expanded to size {bufferSizeInBytes}.");
                }

                IndexBuffer.Dispose();
                IndexBuffer =
                    _device.CreateBuffer(new BufferDescription(BufferType.Index, bufferSizeInBytes, updatable));
            }
            else
            {
                throw new Exception(
                    "Cannot update: Index data is larger than the buffer, dispose and recreate the renderable instead.");
            }
        }

        nint vPtr = _device.MapBuffer(VertexBuffer, MapMode.Write);
        GraphicsUtils.CopyToUnmanaged(vPtr, mesh.Vertices);
        _device.UnmapBuffer(VertexBuffer);

        nint iPtr = _device.MapBuffer(IndexBuffer, MapMode.Write);
        GraphicsUtils.CopyToUnmanaged(iPtr, mesh.Indices);
        _device.UnmapBuffer(IndexBuffer);

        NumElements = (uint) mesh.Indices.Length;
    }
    
    public void Dispose()
    {
        IndexBuffer.Dispose();
        VertexBuffer.Dispose();
    }
}
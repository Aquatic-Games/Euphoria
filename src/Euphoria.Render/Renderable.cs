using System;
using Euphoria.Core;
using grabs.Graphics;
using Buffer = grabs.Graphics.Buffer;

namespace Euphoria.Render;

public sealed class Renderable : IDisposable
{
    internal Buffer VertexBuffer;
    internal Buffer IndexBuffer;

    internal uint NumElements;

    public readonly UpdateFlags UpdateFlags;

    public Material Material;

    internal Renderable(Device device, Buffer vertexBuffer, Buffer indexBuffer, uint numElements, Material material, UpdateFlags updateFlags)
    {
        VertexBuffer = vertexBuffer;
        IndexBuffer = indexBuffer;
        NumElements = numElements;
        Material = material;
        UpdateFlags = updateFlags;
    }

    public Renderable(Mesh mesh, Material material, UpdateFlags updateFlags = UpdateFlags.None)
    {
        Logger.Trace($"Creating renderable from mesh. vSize: {mesh.Vertices.Length} iSize: {mesh.Indices.Length} flags: {updateFlags}");
        
        Device device = Graphics.Device;
        
        bool updatable = (updateFlags & UpdateFlags.Updatable) == UpdateFlags.Updatable;
        
        VertexBuffer = device.CreateBuffer(BufferType.Vertex, mesh.Vertices, updatable);
        IndexBuffer = device.CreateBuffer(BufferType.Index, mesh.Indices, updatable);
        
        NumElements = (uint) mesh.Indices.Length;
        UpdateFlags = updateFlags;
        Material = material;
    }

    public Renderable(uint numVertices, uint numIndices, Material material,
        UpdateFlags updateFlags = UpdateFlags.Updatable)
    {
        Logger.Trace($"Creating empty renderable. vSize: {numVertices} iSize: {numIndices} flags: {updateFlags}");
        
        if ((updateFlags & UpdateFlags.Updatable) != UpdateFlags.Updatable)
            throw new Exception("Empty renderable must be marked with \"Updatable\" flag.");

        Device device = Graphics.Device;

        VertexBuffer =
            device.CreateBuffer(new BufferDescription(BufferType.Vertex, numVertices * Vertex.SizeInBytes, true));
        IndexBuffer =
            device.CreateBuffer(new BufferDescription(BufferType.Index, numIndices * sizeof(uint), true));

        NumElements = numIndices;
        UpdateFlags = updateFlags;
        Material = material;
    }

    public void Update(Mesh mesh)
    {
        Logger.Trace("Updating renderable.");
        
        bool updatable = (UpdateFlags & UpdateFlags.Updatable) == UpdateFlags.Updatable;
        bool recreateBuffers = (UpdateFlags & UpdateFlags.NoRecreateBuffers) != UpdateFlags.NoRecreateBuffers;
        bool expandBuffers = (UpdateFlags & UpdateFlags.ExpandBuffers) == UpdateFlags.ExpandBuffers;
        
        if (!updatable)
            throw new Exception("Cannot update: Renderable has not been created with \"UpdateFlags.Updatable\" flag.");

        Device device = Graphics.Device;

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
                    device.CreateBuffer(new BufferDescription(BufferType.Vertex, bufferSizeInBytes, updatable));
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
                    device.CreateBuffer(new BufferDescription(BufferType.Index, bufferSizeInBytes, updatable));
            }
            else
            {
                throw new Exception(
                    "Cannot update: Index data is larger than the buffer, dispose and recreate the renderable instead.");
            }
        }

        nint vPtr = device.MapBuffer(VertexBuffer, MapMode.Write);
        GraphicsUtils.CopyToUnmanaged(vPtr, mesh.Vertices);
        device.UnmapBuffer(VertexBuffer);

        nint iPtr = device.MapBuffer(IndexBuffer, MapMode.Write);
        GraphicsUtils.CopyToUnmanaged(iPtr, mesh.Indices);
        device.UnmapBuffer(IndexBuffer);

        NumElements = (uint) mesh.Indices.Length;
    }
    
    public void Dispose()
    {
        IndexBuffer.Dispose();
        VertexBuffer.Dispose();
    }
}
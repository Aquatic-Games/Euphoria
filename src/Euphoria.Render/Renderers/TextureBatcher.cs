using System;
using System.Numerics;
using grabs.Graphics;
using Buffer = grabs.Graphics.Buffer;

namespace Euphoria.Render.Renderers;

public class TextureBatcher : IDisposable
{
    public const uint MaxBatchSize = 2048;

    private const uint NumVertices = 4;
    private const uint NumIndices = 6;

    private const uint MaxVertices = NumVertices * MaxBatchSize;
    private const uint MaxIndices = NumIndices * MaxBatchSize;

    private Device _device;
    private CommandList _commandList;
    
    private Buffer _vertexBuffer;
    private Buffer _indexBuffer;

    private Pipeline _pipeline;

    public TextureBatcher(Device device, CommandList commandList)
    {
        _device = device;
        _commandList = commandList;

        _vertexBuffer =
            device.CreateBuffer(new BufferDescription(BufferType.Vertex, MaxVertices * Vertex.SizeInBytes, true));
        _indexBuffer = device.CreateBuffer(new BufferDescription(BufferType.Index, MaxIndices * sizeof(uint), true));
        
        
    }
    
    public void Dispose()
    {
        //_pipeline.Dispose();
        _indexBuffer.Dispose();
        _vertexBuffer.Dispose();
    }

    private readonly struct Vertex
    {
        public readonly Vector2 Position;
        public readonly Vector2 TexCoord;
        public readonly Vector4 Tint;

        public const uint SizeInBytes = 32;
    }
}
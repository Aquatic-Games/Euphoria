﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Euphoria.Core;
using Euphoria.Math;
using grabs.Graphics;
using Buffer = grabs.Graphics.Buffer;
using Color = Euphoria.Math.Color;

namespace Euphoria.Render.Renderers;

public sealed class TextureBatcher : IDisposable
{
    public const uint MaxBatchSize = 2048;

    private const uint NumVertices = 4;
    private const uint NumIndices = 6;

    private const uint MaxVertices = NumVertices * MaxBatchSize;
    private const uint MaxIndices = NumIndices * MaxBatchSize;

    private readonly Vertex[] _vertices;
    private readonly uint[] _indices;
    
    private readonly Buffer _vertexBuffer;
    private readonly Buffer _indexBuffer;

    private readonly Buffer _transformBuffer;

    private readonly Pipeline _pipeline;

    private readonly DescriptorSet _transformSet;

    private readonly List<DrawQueueItem> _drawQueue;

    public TextureBatcher(Device device)
    {
        _vertices = new Vertex[MaxVertices];
        _indices = new uint[MaxIndices];

        Logger.Trace("Creating buffers.");
        
        _vertexBuffer =
            device.CreateBuffer(new BufferDescription(BufferType.Vertex, MaxVertices * Vertex.SizeInBytes, true));
        _indexBuffer = device.CreateBuffer(new BufferDescription(BufferType.Index, MaxIndices * sizeof(uint), true));

        _transformBuffer = device.CreateBuffer(BufferType.Constant, Matrix4x4.Identity, true);

        Logger.Trace("Creating descriptor layouts.");
        
        DescriptorLayout transformLayout = device.CreateDescriptorLayout(
            new DescriptorLayoutDescription(new DescriptorBindingDescription(0, DescriptorType.ConstantBuffer,
                ShaderStage.Vertex)));

        Logger.Trace("Loading shader.");
        
        ShaderModule vTexModule = device.CreateShaderModule(ShaderStage.Vertex,
            ShaderLoader.LoadSpirvShader("Render/Texture", ShaderStage.Vertex), "VSMain");
        ShaderModule pTexModule = device.CreateShaderModule(ShaderStage.Pixel,
            ShaderLoader.LoadSpirvShader("Render/Texture", ShaderStage.Pixel), "PSMain");
        
        Logger.Trace("Creating pipeline.");

        _pipeline = device.CreatePipeline(new PipelineDescription(vTexModule, pTexModule, new[]
            {
                new InputLayoutDescription(Format.R32G32_Float, 0, 0, InputType.PerVertex), // Position
                new InputLayoutDescription(Format.R32G32_Float, 8, 0, InputType.PerVertex), // TexCoord
                new InputLayoutDescription(Format.R32G32B32A32_Float, 16, 0, InputType.PerVertex) // Tint
            }, DepthStencilDescription.Disabled, RasterizerDescription.CullClockwise, BlendDescription.NonPremultiplied,
            [transformLayout, Graphics.TextureDescriptorLayout]));
        
        vTexModule.Dispose();
        pTexModule.Dispose();

        Logger.Trace("Creating descriptors.");
        
        _transformSet =
            device.CreateDescriptorSet(transformLayout, new DescriptorSetDescription(buffer: _transformBuffer));
        
        transformLayout.Dispose();

        _drawQueue = new List<DrawQueueItem>();
    }

    public void Draw(Texture texture, Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight, Color tint, Rectangle<int>? source = null, float sortIndex = 0)
    {
        Rectangle<float> nSource = new Rectangle<float>(0, 0, 1, 1);
        if (source is { } src)
        {
            Size<int> texSize = texture.Size;
            
            nSource.X = src.X / (float) texSize.Width;
            nSource.Y = src.Y / (float) texSize.Height;
            nSource.Width = src.Width / (float) texSize.Width;
            nSource.Height = src.Height / (float) texSize.Height;
        }
        
        _drawQueue.Add(new DrawQueueItem(texture, topLeft, topRight, bottomLeft, bottomRight, nSource, tint, sortIndex));
    }
    
    public void Draw(Texture texture, Vector2 position, Color tint, float rotation, Vector2 scale, Vector2 origin, Rectangle<int>? source = null, float sortIndex = 0)
    {
        Size<float> size = texture.Size.As<float>();
        Matrix4x4 transformMatrix = Matrix4x4.CreateRotationZ(rotation);
        
        Rectangle<float> nSource = new Rectangle<float>(0, 0, 1, 1);
        if (source is { } src)
        {
            nSource.X = src.X / size.Width;
            nSource.Y = src.Y / size.Height;
            nSource.Width = src.Width / size.Width;
            nSource.Height = src.Height / size.Height;
            
            size.Width *= nSource.Width;
            size.Height *= nSource.Height;
        }

        Vector2 topLeft = Vector2.Transform(-origin * scale, transformMatrix) + position;
        Vector2 topRight = Vector2.Transform((-origin + new Vector2(size.Width, 0)) * scale, transformMatrix) + position;
        Vector2 bottomLeft = Vector2.Transform((-origin + new Vector2(0, size.Height)) * scale, transformMatrix) + position;
        Vector2 bottomRight = Vector2.Transform((-origin + new Vector2(size.Width, size.Height)) * scale, transformMatrix) + position;
        
        _drawQueue.Add(new DrawQueueItem(texture, topLeft, topRight, bottomLeft, bottomRight, nSource, tint, sortIndex));
    }

    public void Draw(Texture texture, Vector2 position, Color tint, Rectangle<int>? source = null, float sortIndex = 0)
    {
        Size<float> size = texture.Size.As<float>();
        
        Rectangle<float> nSource = new Rectangle<float>(0, 0, 1, 1);
        if (source is { } src)
        {
            nSource.X = src.X / size.Width;
            nSource.Y = src.Y / size.Height;
            nSource.Width = src.Width / size.Width;
            nSource.Height = src.Height / size.Height;
            
            size.Width *= nSource.Width;
            size.Height *= nSource.Height;
        }

        Vector2 topLeft = position;
        Vector2 topRight = position + new Vector2(size.Width, 0);
        Vector2 bottomLeft = position + new Vector2(0, size.Height);
        Vector2 bottomRight = position + new Vector2(size.Width, size.Height);
        
        _drawQueue.Add(new DrawQueueItem(texture, topLeft, topRight, bottomLeft, bottomRight, nSource, tint, sortIndex));
    }

    public void Draw(Texture texture, Matrix3x2 matrix, Color tint, Rectangle<int>? source = null, float sortIndex = 0)
    {
        Size<float> size = texture.Size.As<float>();
        
        Rectangle<float> nSource = new Rectangle<float>(0, 0, 1, 1);
        if (source is { } src)
        {
            nSource.X = src.X / size.Width;
            nSource.Y = src.Y / size.Height;
            nSource.Width = src.Width / size.Width;
            nSource.Height = src.Height / size.Height;
            
            size.Width *= nSource.Width;
            size.Height *= nSource.Height;
        }

        Vector2 topLeft = Vector2.Transform(Vector2.Zero, matrix);
        Vector2 topRight = Vector2.Transform(new Vector2(size.Width, 0), matrix);
        Vector2 bottomLeft = Vector2.Transform(new Vector2(0, size.Height), matrix);
        Vector2 bottomRight = Vector2.Transform(new Vector2(size.Width, size.Height), matrix);
        
        _drawQueue.Add(new DrawQueueItem(texture, topLeft, topRight, bottomLeft, bottomRight, nSource, tint, sortIndex));
    }

    public void DispatchDrawQueue(CommandList cl, Size<int> viewportSize, Matrix4x4? transform = null, SortMode sortMode = SortMode.Ignore)
    {
        Matrix4x4 cTransform = transform ??
                               Matrix4x4.CreateOrthographicOffCenter(0, viewportSize.Width, viewportSize.Height, 0, -1, 1);
        
        cl.UpdateBuffer(_transformBuffer, 0, cTransform);
        
        uint currentDraw = 0;
        Texture currentTexture = null;

        IEnumerable<DrawQueueItem> drawQueue = sortMode switch
        {
            SortMode.Ignore => _drawQueue,
            SortMode.LowestFirst => _drawQueue.OrderByDescending(item => item.SortIndex),
            SortMode.HighestFirst => _drawQueue.OrderBy(item => item.SortIndex),
            _ => throw new ArgumentOutOfRangeException(nameof(sortMode), sortMode, null)
        };

        foreach (DrawQueueItem item in drawQueue)
        {
            if (item.Texture != currentTexture || currentDraw >= MaxBatchSize)
            {
                FlushVertices(cl, currentDraw, currentTexture?.DescriptorSet);
                currentDraw = 0;
            }

            currentTexture = item.Texture;

            uint vCurrent = currentDraw * NumVertices;
            uint iCurrent = currentDraw * NumIndices;

            Vector2T<float> sourcePos = item.Source.Position;
            Size<float> sourceSize = item.Source.Size;
            Size<float> texSize = currentTexture!.Size.As<float>();

            float texX = sourcePos.X;
            float texY = sourcePos.Y;
            float texW = sourceSize.Width;
            float texH = sourceSize.Height;

            _vertices[vCurrent + 0] = new Vertex(item.TopLeft, new Vector2(texX, texY), item.Tint);
            _vertices[vCurrent + 1] = new Vertex(item.TopRight, new Vector2(texX + texW, texY), item.Tint);
            _vertices[vCurrent + 2] = new Vertex(item.BottomRight, new Vector2(texX + texW, texY + texH), item.Tint);
            _vertices[vCurrent + 3] = new Vertex(item.BottomLeft, new Vector2(texX, texY + texH), item.Tint);

            _indices[iCurrent + 0] = 0 + vCurrent;
            _indices[iCurrent + 1] = 1 + vCurrent;
            _indices[iCurrent + 2] = 3 + vCurrent;
            _indices[iCurrent + 3] = 1 + vCurrent;
            _indices[iCurrent + 4] = 2 + vCurrent;
            _indices[iCurrent + 5] = 3 + vCurrent;

            currentDraw++;
        }
        
        FlushVertices(cl, currentDraw, currentTexture?.DescriptorSet);
        
        // TODO: Probably should NOT clear the draw queue here, not sure.
        _drawQueue.Clear();
    }

    private void FlushVertices(CommandList cl, uint drawCount, DescriptorSet textureSet)
    {
        if (drawCount == 0)
            return;
        
        cl.UpdateBuffer(_vertexBuffer, 0, drawCount * NumVertices * Vertex.SizeInBytes,
            new ReadOnlySpan<Vertex>(_vertices));

        cl.UpdateBuffer(_indexBuffer, 0, drawCount * NumIndices * sizeof(uint), new ReadOnlySpan<uint>(_indices));
        
        cl.SetPipeline(_pipeline);
        
        cl.SetVertexBuffer(0, _vertexBuffer, Vertex.SizeInBytes, 0);
        cl.SetIndexBuffer(_indexBuffer, Format.R32_UInt);
        
        cl.SetDescriptorSet(0, _transformSet);
        cl.SetDescriptorSet(1, textureSet);
        
        cl.DrawIndexed(drawCount * NumIndices);
    }
    
    public void Dispose()
    {
        _pipeline.Dispose();
        _transformBuffer.Dispose();
        _indexBuffer.Dispose();
        _vertexBuffer.Dispose();
    }

    [StructLayout(LayoutKind.Sequential)]
    private readonly struct Vertex
    {
        public readonly Vector2 Position;
        public readonly Vector2 TexCoord;
        public readonly Color Tint;

        public Vertex(Vector2 position, Vector2 texCoord, Color tint)
        {
            Position = position;
            TexCoord = texCoord;
            Tint = tint;
        }

        public const uint SizeInBytes = 32;
    }

    public enum SortMode
    {
        Ignore,
        LowestFirst,
        HighestFirst
    }

    private readonly struct DrawQueueItem
    {
        public readonly Texture Texture;
        public readonly Vector2 TopLeft;
        public readonly Vector2 TopRight;
        public readonly Vector2 BottomLeft;
        public readonly Vector2 BottomRight;
        public readonly Rectangle<float> Source;
        public readonly Color Tint;
        public readonly float SortIndex;

        public DrawQueueItem(Texture texture, Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft,
            Vector2 bottomRight, Rectangle<float> source, Color tint, float sortIndex)
        {
            Texture = texture;
            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
            Source = source;
            Tint = tint;
            SortIndex = sortIndex;
        }
    }
}
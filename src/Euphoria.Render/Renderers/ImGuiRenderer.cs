using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using Euphoria.Core;
using Euphoria.Math;
using grabs.Graphics;
using ImGuiNET;
using Buffer = grabs.Graphics.Buffer;
using ImDrawIdx = ushort;

namespace Euphoria.Render.Renderers;

public class ImGuiRenderer : IDisposable
{
    private Device _device;
    private Size<int> _size;
    
    private readonly IntPtr _context;
    
    private uint _vBufferSize;
    private uint _iBufferSize;
    
    private Buffer _vertexBuffer;
    private Buffer _indexBuffer;
    
    private readonly Pipeline _pipeline;

    private readonly Buffer _projectionBuffer;
    private readonly DescriptorSet _projectionSet;

    private GrabsTexture _imGuiTexture;
    private readonly DescriptorSet _textureSet;

    public readonly Dictionary<string, ImFontPtr> Fonts;

    public IntPtr ImGuiContext => _context;
    
    public unsafe ImGuiRenderer(Device device, Size<int> size)
    {
        _device = device;
        _size = size;
        
        _context = ImGui.CreateContext();
        ImGui.SetCurrentContext(_context);
        
        _vBufferSize = 5000;
        _iBufferSize = 10000;
        
        _vertexBuffer = _device.CreateBuffer(new BufferDescription(BufferType.Vertex, (uint) (_vBufferSize * sizeof(ImDrawVert)), true));
        _indexBuffer = _device.CreateBuffer(new BufferDescription(BufferType.Index, (uint) (_iBufferSize * sizeof(ImDrawIdx)), true));
        
        using ShaderModule vertexModule = _device.CreateShaderModule(ShaderStage.Vertex,
            ShaderLoader.LoadSpirvShader("ImGui", ShaderStage.Vertex), "VSMain");
        using ShaderModule pixelModule = _device.CreateShaderModule(ShaderStage.Pixel,
            ShaderLoader.LoadSpirvShader("ImGui", ShaderStage.Pixel), "PSMain");

        InputLayoutDescription[] inputLayout =
        [
            new InputLayoutDescription(Format.R32G32_Float, 0, 0, InputType.PerVertex), // Position
            new InputLayoutDescription(Format.R32G32_Float, 8, 0, InputType.PerVertex), // TexCoord
            new InputLayoutDescription(Format.R8G8B8A8_UNorm, 16, 0, InputType.PerVertex) // Color
        ];

        using DescriptorLayout projectionLayout = _device.CreateDescriptorLayout(new DescriptorLayoutDescription(
            new DescriptorBindingDescription(0, DescriptorType.ConstantBuffer, ShaderStage.Vertex)));

        using DescriptorLayout textureLayout = _device.CreateDescriptorLayout(new DescriptorLayoutDescription(
            new DescriptorBindingDescription(0, DescriptorType.Texture, ShaderStage.Pixel)));

        PipelineDescription pipelineDesc = new PipelineDescription(vertexModule, pixelModule, inputLayout,
            DepthStencilDescription.Disabled, RasterizerDescription.CullNone, BlendDescription.NonPremultiplied,
            [projectionLayout, textureLayout]);

        _pipeline = _device.CreatePipeline(pipelineDesc);

        _projectionBuffer = _device.CreateBuffer(BufferType.Constant, Matrix4x4.Identity, true);
        _projectionSet = _device.CreateDescriptorSet(projectionLayout, new DescriptorSetDescription(buffer: _projectionBuffer));

        _textureSet = _device.CreateDescriptorSet(textureLayout);
        RecreateFontTexture();

        ImGuiIOPtr io = ImGui.GetIO();
        io.DisplaySize = new Vector2(size.Width, size.Height);
        
        io.Fonts.AddFontDefault();
        io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

        Fonts = new Dictionary<string, ImFontPtr>();
    }

    public void AddFont(string path, uint size, string name)
    {
        ImFontPtr font = ImGui.GetIO().Fonts.AddFontFromFileTTF(path, size);
        Fonts.Add(name, font);
        RecreateFontTexture();
    }

    internal unsafe void Render(CommandList cl, Framebuffer framebuffer)
    {
        ImGui.SetCurrentContext(_context);
        
        ImGui.Render();
        ImDrawDataPtr drawData = ImGui.GetDrawData();

        if (drawData.TotalVtxCount >= _vBufferSize)
        {
            Logger.Trace("Recreate vertex buffer.");
            _vertexBuffer.Dispose();
            _vBufferSize = (uint) (drawData.TotalVtxCount + 5000);
            _vertexBuffer = _device.CreateBuffer(new BufferDescription(BufferType.Vertex,
                (uint) (_vBufferSize * sizeof(ImDrawVert)), true));
        }

        if (drawData.TotalIdxCount >= _iBufferSize)
        {
            Logger.Trace("Recreate index buffer.");
            _indexBuffer.Dispose();
            _iBufferSize = (uint) (drawData.TotalIdxCount + 10000);
            _indexBuffer = _device.CreateBuffer(new BufferDescription(BufferType.Index, 
                (uint) (_iBufferSize * sizeof(ImDrawIdx)), true));
        }

        uint vertexOffset = 0;
        uint indexOffset = 0;
        nint vPtr = _device.MapBuffer(_vertexBuffer, MapMode.Write);
        nint iPtr = _device.MapBuffer(_indexBuffer, MapMode.Write);
        for (int i = 0; i < drawData.CmdListsCount; i++)
        {
            ImDrawListPtr cmdList = drawData.CmdLists[i];
            
            Unsafe.CopyBlock((byte*) vPtr + vertexOffset, (void*) cmdList.VtxBuffer.Data, (uint) (cmdList.VtxBuffer.Size * sizeof(ImDrawVert)));
            Unsafe.CopyBlock((byte*) iPtr + indexOffset, (void*) cmdList.IdxBuffer.Data, (uint) (cmdList.IdxBuffer.Size * sizeof(ImDrawIdx)));

            vertexOffset += (uint) (cmdList.VtxBuffer.Size * sizeof(ImDrawVert));
            indexOffset += (uint) (cmdList.IdxBuffer.Size * sizeof(ImDrawIdx));
        }
        _device.UnmapBuffer(_vertexBuffer);
        _device.UnmapBuffer(_indexBuffer);

        cl.UpdateBuffer(_projectionBuffer, 0,
            Matrix4x4.CreateOrthographicOffCenter(drawData.DisplayPos.X, drawData.DisplayPos.X + drawData.DisplaySize.X,
                drawData.DisplayPos.Y + drawData.DisplaySize.Y, drawData.DisplayPos.Y, -1, 1));
        
        cl.BeginRenderPass(new RenderPassDescription(framebuffer, Vector4.Zero, LoadOp.Load));

        cl.SetViewport(new Viewport(0, 0, (uint) drawData.DisplaySize.X, (uint) drawData.DisplaySize.Y));
        
        // TODO: IMPORTANT!!! In GRABS, setting the pipeline AFTER the vertex/index buffer causes it to fail,
        // as the VAO bind overwrites the currently bound Vertex and Index buffers.
        cl.SetPipeline(_pipeline);
        
        cl.SetVertexBuffer(0, _vertexBuffer, (uint) sizeof(ImDrawVert), 0);
        cl.SetIndexBuffer(_indexBuffer, Format.R16_UInt);
        
        cl.SetDescriptorSet(0, _projectionSet);

        vertexOffset = 0;
        indexOffset = 0;
        Vector2 clipOff = drawData.DisplayPos;
        for (int i = 0; i < drawData.CmdListsCount; i++)
        {
            ImDrawListPtr cmdList = drawData.CmdLists[i];

            for (int j = 0; j < cmdList.CmdBuffer.Size; j++)
            {
                ImDrawCmdPtr drawCmd = cmdList.CmdBuffer[j];

                // TODO: Handle user pointer?
                if (drawCmd.UserCallback != IntPtr.Zero)
                    continue;

                DescriptorSet texSet = _textureSet;
                if (drawCmd.TextureId != 0)
                    texSet = Texture.GetTexture((ulong) drawCmd.TextureId).DescriptorSet;

                Vector2 clipMin = new Vector2(drawCmd.ClipRect.X - clipOff.X, drawCmd.ClipRect.Y - clipOff.Y);
                Vector2 clipMax = new Vector2(drawCmd.ClipRect.Z - clipOff.X, drawCmd.ClipRect.W - clipOff.Y);
                
                if (clipMax.X <= clipMin.X || clipMax.Y <= clipMin.Y)
                    continue;

                cl.SetScissor(new Rectangle((int) clipMin.X, (int) clipMin.Y, (int) clipMax.X - (int) clipMin.X, (int) clipMax.Y - (int) clipMin.Y));
                
                cl.SetDescriptorSet(1, texSet);
                cl.DrawIndexed(drawCmd.ElemCount, drawCmd.IdxOffset + indexOffset,
                    (int) (drawCmd.VtxOffset + vertexOffset));
            }

            vertexOffset += (uint) cmdList.VtxBuffer.Size;
            indexOffset += (uint) cmdList.IdxBuffer.Size;
        }
        
        cl.EndRenderPass();
    }

    internal void Resize(in Size<int> size)
    {
        _size = size;
        ImGui.GetIO().DisplaySize = new Vector2(size.Width, size.Height);
    }
    
    private unsafe void RecreateFontTexture()
    {
        _imGuiTexture?.Dispose();

        ImGuiIOPtr io = ImGui.GetIO();
        io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height);

        TextureDescription textureDesc = TextureDescription.Texture2D((uint) width, (uint) height, 1,
            Format.R8G8B8A8_UNorm, TextureUsage.ShaderResource);
        _imGuiTexture = _device.CreateTexture(textureDesc, (void**) &pixels);
        
        _device.UpdateDescriptorSet(_textureSet, new DescriptorSetDescription(texture: _imGuiTexture));
        
        io.Fonts.SetTexID(0);
    }
    
    public void Dispose()
    {
        _projectionSet.Dispose();
        _projectionBuffer.Dispose();
        _pipeline.Dispose();
        _indexBuffer.Dispose();
        _vertexBuffer.Dispose();
        
        ImGui.DestroyContext(_context);
    }
}
using System;
using System.Drawing;
using System.Numerics;
using grabs.Graphics;
using ImGuiNET;
using Buffer = grabs.Graphics.Buffer;
using ImDrawIdx = ushort;

namespace Euphoria.Render.Renderers;

public class ImGuiRenderer : IDisposable
{
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
    
    public unsafe ImGuiRenderer(Device device)
    {
        _context = ImGui.CreateContext();
        ImGui.SetCurrentContext(_context);
        
        _vBufferSize = 5000;
        _iBufferSize = 10000;
        
        _vertexBuffer = device.CreateBuffer(new BufferDescription(BufferType.Vertex, (uint) (_vBufferSize * sizeof(ImDrawVert)), true));
        _indexBuffer = device.CreateBuffer(new BufferDescription(BufferType.Index, (uint) (_iBufferSize * sizeof(ImDrawIdx)), true));
        
        using ShaderModule vertexModule = device.CreateShaderModule(ShaderStage.Vertex,
            ShaderLoader.LoadSpirvShader("ImGui", ShaderStage.Vertex), "VSMain");
        using ShaderModule pixelModule = device.CreateShaderModule(ShaderStage.Pixel,
            ShaderLoader.LoadSpirvShader("ImGui", ShaderStage.Pixel), "PSMain");

        InputLayoutDescription[] inputLayout =
        [
            new InputLayoutDescription(Format.R32G32_Float, 0, 0, InputType.PerVertex), // Position
            new InputLayoutDescription(Format.R32G32_Float, 8, 0, InputType.PerVertex), // TexCoord
            new InputLayoutDescription(Format.R8G8B8A8_UNorm, 16, 0, InputType.PerVertex) // Color
        ];

        using DescriptorLayout projectionLayout = device.CreateDescriptorLayout(new DescriptorLayoutDescription(
            new DescriptorBindingDescription(0, DescriptorType.ConstantBuffer, ShaderStage.Vertex)));

        using DescriptorLayout textureLayout = device.CreateDescriptorLayout(new DescriptorLayoutDescription(
            new DescriptorBindingDescription(0, DescriptorType.Texture, ShaderStage.Pixel)));

        PipelineDescription pipelineDesc = new PipelineDescription(vertexModule, pixelModule, inputLayout,
            DepthStencilDescription.Disabled, RasterizerDescription.CullNone, BlendDescription.NonPremultiplied,
            [projectionLayout, textureLayout]);

        _pipeline = device.CreatePipeline(pipelineDesc);

        _projectionBuffer = device.CreateBuffer(BufferType.Constant, Matrix4x4.Identity, true);
        _projectionSet = device.CreateDescriptorSet(projectionLayout, new DescriptorSetDescription(buffer: _projectionBuffer));

        _textureSet = device.CreateDescriptorSet(textureLayout);
        RecreateFontTexture(device);

        ImGuiIOPtr io = ImGui.GetIO();
        io.Fonts.AddFontDefault();
        io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
    }

    private unsafe void RecreateFontTexture(Device device)
    {
        _imGuiTexture?.Dispose();

        ImGuiIOPtr io = ImGui.GetIO();
        io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height);

        TextureDescription textureDesc = TextureDescription.Texture2D((uint) width, (uint) height, 1,
            Format.R8G8B8A8_UNorm, TextureUsage.ShaderResource);
        _imGuiTexture = device.CreateTexture(textureDesc, (void*) pixels);
        
        device.UpdateDescriptorSet(_textureSet, new DescriptorSetDescription(texture: _imGuiTexture));
        
        io.Fonts.SetTexID(0);
    }

    internal unsafe void Render(Device device, CommandList cl, Framebuffer framebuffer)
    {
        ImGui.SetCurrentContext(_context);

        ImGui.NewFrame();

        ImGui.GetIO().DisplaySize = new Vector2(1280, 720);
        ImGui.GetIO().DisplayFramebufferScale = Vector2.One;
        
        ImGui.ShowDemoWindow();
        
        ImGui.Render();
        ImDrawDataPtr drawData = ImGui.GetDrawData();

        if (drawData.TotalVtxCount >= _vBufferSize)
        {
            _vertexBuffer.Dispose();
            _vBufferSize = (uint) (drawData.TotalVtxCount + 5000);
            _vertexBuffer = device.CreateBuffer(new BufferDescription(BufferType.Vertex,
                (uint) (_vBufferSize * sizeof(ImDrawVert)), true));
        }

        if (drawData.TotalIdxCount >= _iBufferSize)
        {
            _indexBuffer.Dispose();
            _iBufferSize = (uint) (drawData.TotalIdxCount + 10000);
            _indexBuffer = device.CreateBuffer(new BufferDescription(BufferType.Index, 
                (uint) (_iBufferSize * sizeof(ImDrawIdx)), true));
        }

        uint vertexOffset = 0;
        uint indexOffset = 0;
        for (int i = 0; i < drawData.CmdListsCount; i++)
        {
            ImDrawListPtr cmdList = drawData.CmdLists[i];
            
            cl.UpdateBuffer(_vertexBuffer, vertexOffset, (uint) (cmdList.VtxBuffer.Size * sizeof(ImDrawVert)), (void*) cmdList.VtxBuffer.Data);
            cl.UpdateBuffer(_indexBuffer, indexOffset, (uint) (cmdList.IdxBuffer.Size * sizeof(ImDrawIdx)), (void*) cmdList.IdxBuffer.Data);

            vertexOffset += (uint) (cmdList.VtxBuffer.Size * sizeof(ImDrawVert));
            indexOffset += (uint) (cmdList.IdxBuffer.Size * sizeof(ImDrawIdx));
        }

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

                if (drawCmd.TextureId != 0)
                    throw new NotImplementedException("Multiple textures are not supported right now.");

                Vector2 clipMin = new Vector2(drawCmd.ClipRect.X - clipOff.X, drawCmd.ClipRect.Y - clipOff.Y);
                Vector2 clipMax = new Vector2(drawCmd.ClipRect.Z - clipOff.X, drawCmd.ClipRect.W - clipOff.Y);
                
                if (clipMax.X <= clipMin.X || clipMax.Y <= clipMin.Y)
                    continue;

                cl.SetScissor(new Rectangle((int) clipMin.X, (int) clipMin.Y, (int) clipMax.X, (int) clipMax.Y));
                
                cl.SetDescriptorSet(1, _textureSet);
                cl.DrawIndexed(drawCmd.ElemCount, drawCmd.IdxOffset + indexOffset,
                    (int) (drawCmd.VtxOffset + vertexOffset));
            }

            vertexOffset += (uint) cmdList.VtxBuffer.Size;
            indexOffset += (uint) cmdList.IdxBuffer.Size;
        }
        
        cl.EndRenderPass();
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
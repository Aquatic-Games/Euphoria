using System;
using System.Collections.Generic;
using System.Numerics;
using Euphoria.Math;
using Euphoria.Render;
using grabs.Graphics;
using ImGuiNET;
using Texture = Euphoria.Render.Texture;

namespace Euphoria.Engine.Debugging;

public class RendererTab : IDebugTab
{
    public string TabName => "Renderer";
    
    private List<Texture> _viewTextures;

    public RendererTab()
    {
        _viewTextures = new List<Texture>();
        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
    }
    
    public void Update()
    {
        Size<int> constSize = new Size<int>(1280, 720);

        Vector2 uv0 = new Vector2(0, 0);
        Vector2 uv1 = new Vector2(1, 1);

        if (Graphics.Api is GraphicsApi.OpenGL or GraphicsApi.OpenGLES)
        {
            uv0 = new Vector2(0, 1);
            uv1 = new Vector2(1, 0);
            
            ImGui.Text("*OpenGL renderer - Textures have been flipped.");
        }

        if (ImGui.CollapsingHeader("Renderer"))
        {
            foreach ((string name, Texture texture) in Graphics.Renderer3D.GetDebugTextures())
            {
                ImGui.Image((nint) texture.Id, GetScaledSize(texture.Size, constSize) / 4, uv0, uv1);
                ImGui.SameLine();
                ImGui.Text($"{name}\n{texture.Size}");
            }
        }

        if (ImGui.CollapsingHeader("Textures"))
        {
            int i = 0;
            foreach (Texture texture in Texture.GetAllTextures())
            {
                if (ImGui.ImageButton($"{texture.Id}", (nint) texture.Id, GetScaledSize(texture.Size, constSize) / 8, uv0, uv1))
                    _viewTextures.Add(texture);
                
                ImGui.SameLine();
                ImGui.Text($"Texture {texture.Id}\n{texture.Size}\n{texture.Format}");
                
                if (i++ % 2 == 0)
                    ImGui.SameLine();
            }
        }
        
        for (int i = 0; i < _viewTextures.Count; i++)
        {
            Texture texture = _viewTextures[i];
            
            bool open = true;
            if (ImGui.Begin($"View Texture {texture.Id}##{texture.Id}", ref open))
            {
                Vector2 size = ImGui.GetWindowSize();
                ImGui.Image((nint) texture.Id, GetScaledSize(texture.Size, new Size<int>((int) size.X, (int) size.Y)));
                ImGui.End();
            }

            if (!open)
                _viewTextures.Remove(texture);
        }
        
    }

    private static Vector2 GetScaledSize(Size<int> actualSize, Size<int> maxSize)
    {
        Vector2 size = new Vector2(actualSize.Width, actualSize.Height);
        if (actualSize.Width >= maxSize.Width)
            size *= maxSize.Width / size.X;
        else if (actualSize.Height >= maxSize.Height)
            size *= maxSize.Height / size.Y;
        else
        {
            if (actualSize.Width >= actualSize.Height)
                size *= maxSize.Width / size.X;
            else
                size *= maxSize.Height / size.Y;
        }

        return size;
    }
}
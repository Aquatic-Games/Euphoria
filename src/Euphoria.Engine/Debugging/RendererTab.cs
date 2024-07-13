using System.Numerics;
using Euphoria.Render;
using grabs.Graphics;
using ImGuiNET;
using Texture = Euphoria.Render.Texture;

namespace Euphoria.Engine.Debugging;

public class RendererTab : IDebugTab
{
    public string TabName => "Renderer";
    
    public void Update()
    {
        Vector2 constSize = new Vector2(1280, 720);

        Vector2 uv0 = new Vector2(0, 0);
        Vector2 uv1 = new Vector2(1, 1);

        if (Graphics.Api is GraphicsApi.OpenGL or GraphicsApi.OpenGLES)
        {
            uv0 = new Vector2(0, 1);
            uv1 = new Vector2(1, 0);
            
            ImGui.Text("*OpenGL renderer - Textures have been flipped.");
        }
        
        foreach ((string name, Texture texture) in Graphics.Renderer3D.GetDebugTextures())
        {
            Vector2 size = new Vector2(texture.Size.Width, texture.Size.Height);
            if (size.X > constSize.X)
                size *= constSize.X / size.X;
            if (size.Y >= constSize.Y)
                size *= constSize.Y / size.Y;
            
            ImGui.Image((nint) texture.Id, size / 4, uv0, uv1);
            ImGui.SameLine();
            ImGui.Text($"{name}\n{texture.Size}");
        }
    }
}
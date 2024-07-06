using System.Numerics;
using Euphoria.Render;
using ImGuiNET;

namespace Euphoria.Engine.Debugging;

public class RendererTab : IDebugTab
{
    public string TabName => "Renderer";
    
    public void Update()
    {
        Vector2 constSize = new Vector2(1280, 720);
        
        foreach ((string name, Texture texture) in Graphics.Renderer3D.GetDebugTextures())
        {
            Vector2 size = new Vector2(texture.Size.Width, texture.Size.Height);
            if (size.X > constSize.X)
                size *= constSize.X / size.X;
            if (size.Y >= constSize.Y)
                size *= constSize.Y / size.Y;
            
            ImGui.Image((nint) texture.Id, size / 4);
            ImGui.SameLine();
            ImGui.Text($"{name}\n{texture.Size}");
        }
    }
}
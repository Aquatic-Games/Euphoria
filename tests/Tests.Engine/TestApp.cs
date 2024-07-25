using Euphoria.Engine;
using Euphoria.Engine.Debugging;
using Euphoria.Engine.Scenes;
using Euphoria.Render;
using Euphoria.Render.Renderers;
using ImGuiNET;

namespace Tests.Engine;

public class TestApp : Application
{
    public const string FileBase = "/home/aqua";
    
    public override void Initialize(Scene initialScene)
    {
        base.Initialize(initialScene);
        
        ImGuiRenderer imGui = Graphics.ImGuiRenderer;
        imGui.AddFont($"{FileBase}/Downloads/Russo_One/RussoOne-Regular.ttf", 14, "RussoOne");

        EuphoriaDebug.IsOpen = true;
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        
        if (Input.IsKeyPressed(Key.Escape))
            App.Close();
    }
}
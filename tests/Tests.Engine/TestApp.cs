using Euphoria.Engine;
using Euphoria.Engine.Scenes;
using Euphoria.Render.Renderers;
using ImGuiNET;

namespace Tests.Engine;

public class TestApp : Application
{
    public override void Initialize(Scene initialScene)
    {
        base.Initialize(initialScene);
        
        ImGuiRenderer imGui = App.Graphics.ImGuiRenderer;
            
        imGui.AddFont(@"C:\Users\ollie\Downloads\Russo_One\RussoOne-Regular.ttf", 14, "RussoOne");
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        
        if (Input.IsKeyPressed(Key.Escape))
            App.Close();
    }
}
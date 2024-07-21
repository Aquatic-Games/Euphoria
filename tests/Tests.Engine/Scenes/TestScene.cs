using System;
using System.Numerics;
using Euphoria.Engine;
using Euphoria.Engine.Entities;
using Euphoria.Engine.Scenes;
using Euphoria.Math;
using Euphoria.Render;
using ImGuiNET;
using Tests.Engine.Components;

namespace Tests.Engine.Scenes;

public class TestScene : Scene
{
    private Texture _texture;
    
    public override void Initialize()
    {
        App.TargetFramesPerSecond = 0;

        _texture = new Texture("C:/Users/ollie/Pictures/awesomeface.png");

        Entity entity = new Entity("test");
        entity.AddComponent(new SpriteComponent(new Texture("C:/Users/ollie/Pictures/BAGELMIP.png")));
        entity.AddComponent(new MoveComponent());
        //entity.AddComponent(new MoveComponent());
        
        AddEntity(entity);
        //AddEntity(entity);
        
        base.Initialize();
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        
        if (Input.IsKeyPressed(Key.P))
            SceneManager.LoadAndSwitchScene(new Scene3D());

        ImGui.PushFont(Graphics.ImGuiRenderer.Fonts["RussoOne"]);
        if (ImGui.Begin("Hello"))
        {
            ImGui.Image((IntPtr) _texture.Id, new Vector2(128));
            ImGui.Button("ASDASD");
            ImGui.End();
        }

        ImGui.PopFont();
    }

    public override void Draw()
    {
        Graphics.TextureBatcher.Draw(_texture, new Vector2(0, 0), Color.White);
        
        base.Draw();
    }
}
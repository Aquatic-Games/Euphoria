using System;
using System.Numerics;
using Euphoria.Engine;
using Euphoria.Engine.Entities;
using Euphoria.Engine.Scenes;
using Euphoria.Math;
using Euphoria.Render;
using Tests.Engine.Components;

namespace Tests.Engine.Scenes;

public class TestScene : Scene
{
    private Texture _texture;
    
    public override void Initialize()
    {
        App.TargetFramesPerSecond = 0;
        Graphics graphics = App.Graphics;

        _texture = graphics.CreateTexture(new Bitmap(@"C:\Users\ollie\Pictures\awesomeface.png"));

        Entity entity = new Entity("test");
        entity.AddComponent(new SpriteComponent(graphics.CreateTexture(new Bitmap(@"C:\Users\ollie\Pictures\BAGELMIP.png"))));
        entity.AddComponent(new MoveComponent());
        //entity.AddComponent(new MoveComponent());
        
        AddEntity(entity);
        //AddEntity(entity);
        
        base.Initialize();
    }

    public override void Draw()
    {
        Graphics graphics = App.Graphics;
        graphics.TextureBatcher.Draw(_texture, new Vector2(0, 0), Color.White);
        
        base.Draw();
    }
}
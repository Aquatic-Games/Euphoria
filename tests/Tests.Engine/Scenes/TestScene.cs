using System.Numerics;
using Euphoria.Engine;
using Euphoria.Engine.Scenes;
using Euphoria.Math;
using Euphoria.Render;

namespace Tests.Engine.Scenes;

public class TestScene : Scene
{
    private Texture _texture;
    
    public override void Initialize()
    {
        Graphics graphics = App.Graphics;

        _texture = graphics.CreateTexture(new Bitmap(@"C:\Users\ollie\Pictures\awesomeface.png"));
        
        base.Initialize();
    }

    public override void Draw()
    {
        base.Draw();
        
        Graphics graphics = App.Graphics;
        graphics.TextureBatcher.Draw(_texture, new Vector2(0, 0), Color.White);
    }
}
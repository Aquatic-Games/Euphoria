using System.Numerics;
using Euphoria.Render;
using Euphoria.Render.Renderers;
using u4.Engine;
using u4.Math;

namespace u4;

public class TestApp : Application
{
    private Texture _texture;
    private float _value;
    
    public override void Initialize()
    {
        base.Initialize();

        _texture = Graphics.CreateTexture(new Bitmap(@"C:\Users\ollie\Pictures\awesomeface.png"));
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        _value += dt;
    }

    public override void Draw()
    {
        base.Draw();
        
        Graphics.Renderer2D.DrawSprite(new Sprite(_texture, new Vector3(0, 0, 0)));
        
        Graphics.TextureBatcher.Draw(_texture, new Vector2(100), Color.White, _value, Vector2.One, Vector2.Zero);
    }
}
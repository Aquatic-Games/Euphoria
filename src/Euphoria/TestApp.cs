using System.Numerics;
using Euphoria.Render;
using u4.Engine;

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
        
        Graphics.TextureBatcher.Draw(_texture, new Vector2(float.Sin(_value * 4) * 200 + 200, 50), Vector4.One);
    }
}
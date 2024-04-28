using System.IO;
using System.Numerics;
using Euphoria.Render;
using Euphoria.Render.Renderers;
using grabs.Graphics;
using u4.Engine;
using u4.Math;
using Texture = Euphoria.Render.Texture;

namespace u4;

public class TestApp : Application
{
    private Texture _texture;
    private Texture _texture2;
    private float _value;
    
    public override void Initialize()
    {
        base.Initialize();

        DDS dds = DDS.FromFile("Content/testdds.dds");

        _texture = Graphics.CreateTexture(new Bitmap(@"C:\Users\ollie\Pictures\awesomeface.png"));
        _texture2 = Graphics.CreateTexture(new Bitmap(File.ReadAllBytes("Content/testdds.bin"), new Size<int>(512, 512), Format.R8G8B8A8_UNorm));
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        _value += dt;
    }

    public override void Draw()
    {
        base.Draw();
        
        Graphics.Renderer2D.DrawSprite(new Sprite(_texture, new Vector3(0, 0, 0), _value));
        Graphics.Renderer2D.DrawSprite(new Sprite(_texture2, new Vector3(0, 0, 0), _value * 0.5f));
        
        Graphics.TextureBatcher.Draw(_texture, new Vector2(100), Color.White, _value, Vector2.One, Vector2.Zero);
        Graphics.TextureBatcher.Draw(_texture2, new Vector2(0), Color.White);
    }
}
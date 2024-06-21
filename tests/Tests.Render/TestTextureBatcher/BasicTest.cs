using System.Numerics;
using Euphoria.Render;
using Euphoria.Render.Renderers;
using u4.Math;

namespace Tests.Render.TestTextureBatcher;

public class BasicTest : TestBase
{
    private Texture _texture;
    
    protected override void Initialize()
    {
        _texture = Graphics.CreateTexture(new Bitmap("Content/awesomeface.png"));
    }

    protected override void Draw()
    {
        TextureBatcher batcher = Graphics.TextureBatcher;
        
        batcher.Draw(_texture, new Vector2(0), Color.White);
        batcher.Draw(_texture, new Vector2(50), Color.White);
        batcher.Draw(_texture, new Vector2(100), Color.White);
        batcher.Draw(_texture, new Vector2(150), Color.White);
        batcher.Draw(_texture, new Vector2(200), Color.White);
        batcher.Draw(_texture, new Vector2(250), Color.White);
        batcher.Draw(_texture, new Vector2(300), Color.White);
    }

    public BasicTest() : base("Texture Batcher Basic Test") { }
}
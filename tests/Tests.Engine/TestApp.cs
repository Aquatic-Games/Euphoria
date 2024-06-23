using System.Numerics;
using Euphoria.Engine;
using Euphoria.Math;
using Euphoria.Render;
using Euphoria.Render.Renderers;

namespace Tests.Engine;

public class TestApp : Application
{
    private Texture _texture;
    
    public override void Initialize()
    {
        base.Initialize();

        _texture = Graphics.CreateTexture(new Bitmap("Content/awesomeface.png"));
    }

    public override void Draw()
    {
        base.Draw();

        TextureBatcher batcher = Graphics.TextureBatcher;
        
        batcher.Draw(_texture, new Vector2(0, 0), Color.White);
    }

    public override void Dispose()
    {
        base.Dispose();
        
        _texture.Dispose();
    }
}
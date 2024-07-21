using System.Numerics;
using Euphoria.Math;
using Euphoria.Render;
using Euphoria.Render.Renderers;
using grabs.Graphics;
using Texture = Euphoria.Render.Texture;

namespace Tests.Render.TestTextureBatcher;

public class SamplerTest : TestBase
{
    private Texture _defaultTexture;
    private Texture _pointTexture;
    private Texture _anisoTexture;
    
    protected override void Initialize()
    {
        base.Initialize();

        const string path = @"C:\Users\ollie\Pictures\ball.png";

        _defaultTexture = new Texture(path);
        _pointTexture = new Texture(path, SamplerDescription.PointClamp);
        _anisoTexture = new Texture(path, SamplerDescription.Anisotropic16x);
    }

    protected override void Draw()
    {
        TextureBatcher batcher = Graphics.TextureBatcher;

        const float w = 200;
        const float h = 200;
        
        batcher.Draw(_defaultTexture, new Vector2(0, 0), new Vector2(w, 0), new Vector2(0, h), new Vector2(w, h), Color.White);
        batcher.Draw(_pointTexture, new Vector2(w + 0, 0), new Vector2(w + w, 0), new Vector2(w, h), new Vector2(w + w, h), Color.White);
    }

    public SamplerTest() : base("Sampler Test") { }
}
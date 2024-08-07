﻿using System.Numerics;
using Euphoria.Math;
using Euphoria.Render;
using Euphoria.Render.Renderers;

namespace Tests.Render.TestTextureBatcher;

public class BasicTest : TestBase
{
    private Texture _texture;
    
    protected override void Initialize()
    {
        _texture = new Texture("Content/awesomeface.png");
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

    public override void Dispose()
    {
        _texture.Dispose();
        
        base.Dispose();
    }

    public BasicTest() : base("Texture Batcher Basic Test") { }
}
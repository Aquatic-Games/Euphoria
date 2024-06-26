using System.Numerics;
using Euphoria.Engine;
using Euphoria.Engine.Scenes;
using Euphoria.Math;
using Euphoria.Render;
using Euphoria.Render.Renderers;
using Tests.Engine.Scenes;

namespace Tests.Engine;

public class TestApp : Application
{
    private Scene _scene;
    
    public override void Initialize()
    {
        base.Initialize();

        _scene = new TestScene();
        
        _scene.Initialize();
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        
        _scene.Update(dt);
    }

    public override void Draw()
    {
        base.Draw();
        
        _scene.Draw();
    }

    public override void Dispose()
    {
        base.Dispose();
        
        _scene.Dispose();
    }
}
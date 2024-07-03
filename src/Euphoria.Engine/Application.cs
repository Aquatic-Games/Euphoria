using System;
using Euphoria.Engine.Scenes;
using Euphoria.Render;

namespace Euphoria.Engine;

public class Application : IDisposable
{
    public Graphics Graphics => App.Graphics;
    
    public virtual void Initialize(Scene initialScene)
    {
        SceneManager.Initialize(initialScene);
    }

    public virtual void Update(float dt)
    {
        SceneManager.Update(dt);
    }

    public virtual void Draw()
    {
        SceneManager.Draw();
    }

    public virtual void Dispose()
    {
        SceneManager.Unload();
    }
}
using System;
using Euphoria.Render;

namespace Euphoria.Engine;

public class Application : IDisposable
{
    public Window Window => App.Window;
    public Graphics Graphics => App.Graphics;
    
    public virtual void Initialize() { }

    public virtual void Update(float dt) { }

    public virtual void Draw() { }

    public virtual void Dispose() { }
}
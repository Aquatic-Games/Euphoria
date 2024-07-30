using System;
using Euphoria.Render;

namespace Euphoria.Engine.Entities.Components;

public abstract class Component : IDisposable
{
    public Entity Entity;

    public ref Transform Transform => ref Entity.Transform;
    
    public virtual void Initialize() { }

    public virtual void Tick(float dt) { }

    public virtual void Update(float dt) { }

    public virtual void Draw() { }

    public virtual void Dispose() { }
}
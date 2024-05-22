using System;

namespace u4.Engine.Entities;

public class Entity : IDisposable
{
    public readonly string Name;

    public Transform Transform;

    public Entity(string name) : this(name, new Transform()) { }

    public Entity(string name, Transform transform)
    {
        Name = name;
        Transform = transform;
    }

    public virtual void Initialize() { }

    public virtual void Update(float dt) { }

    public virtual void Draw() { }

    public virtual void Dispose() { }
}
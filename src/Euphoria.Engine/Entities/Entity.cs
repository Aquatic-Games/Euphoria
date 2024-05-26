using System;
using System.Collections.Generic;
using u4.Engine.Entities.Components;

namespace u4.Engine.Entities;

public class Entity : IDisposable
{
    private Dictionary<Type, Component> _components;
    private bool _hasInitialized;
    
    public readonly string Name;

    public Transform Transform;

    public Entity(string name) : this(name, new Transform()) { }

    public Entity(string name, Transform transform)
    {
        Name = name;
        Transform = transform;

        _components = new Dictionary<Type, Component>();
        _hasInitialized = false;
    }

    public bool TryAddComponent(Component component)
    {
        return _components.TryAdd(component.GetType(), component);
    }

    public void AddComponent(Component component)
    {
        if (!TryAddComponent(component))
            throw new Exception($"A component with type {component.GetType()} has already been added to the entity.");
    }
    
    

    public virtual void Initialize()
    {
        foreach ((_, Component component) in _components)
            component.Initialize();
    }

    public virtual void Update(float dt)
    {
        foreach ((_, Component component) in _components)
            component.Update(dt);
    }

    public virtual void Draw()
    {
        foreach ((_, Component component) in _components)
            component.Draw();
    }

    public virtual void Dispose()
    {
        foreach ((_, Component component) in _components)
            component.Dispose();
    }
}
using System;
using System.Collections.Generic;
using Euphoria.Engine.Entities.Components;

namespace Euphoria.Engine.Entities;

public class Entity : IDisposable
{
    private List<Component> _components;
    private Dictionary<Type, Component> _componentPointers;
    private bool _hasInitialized;

    public readonly string Name;

    public Transform Transform;

    public Entity(string name, Transform transform)
    {
        Name = name;
        Transform = transform;

        _components = new List<Component>();
        _componentPointers = new Dictionary<Type, Component>();
    }

    public bool TryAddComponent(Component component)
    {
        Type componentType = component.GetType();

        if (!_componentPointers.TryAdd(componentType, component))
            return false;
        
        _components.Add(component);

        component.Entity = this;
        if (_hasInitialized)
            component.Initialize();

        return true;
    }

    public void AddComponent(Component component)
    {
        if (!TryAddComponent(component))
            throw new Exception($"Component of type {component.GetType()} has already been added to the entity.");
    }

    public virtual void Initialize()
    {
        if (_hasInitialized)
            return;

        _hasInitialized = true;
        
        foreach (Component component in _components)
            component.Initialize();
    }

    public virtual void Update(float dt)
    {
        foreach (Component component in _components)
            component.Update(dt);
    }

    public virtual void Draw()
    {
        foreach (Component component in _components)
            component.Draw();
    }

    public virtual void Dispose()
    {
        foreach (Component component in _components)
            component.Dispose();
    }
}
using System;
using System.Collections.Generic;
using Euphoria.Core;
using Euphoria.Engine.Entities;

namespace Euphoria.Engine.Scenes;

public abstract class Scene : IDisposable
{
    // Use that Entities are heap allocated to our advantage.
    // The List is what is iterated through, as its iteration performance is fast.
    // The dictionary is what is used for lookup.
    private List<Entity> _entities;
    private Dictionary<string, Entity> _entityPointers;
    private bool _hasInitialized;

    public Scene()
    {
        _entities = new List<Entity>();
        _entityPointers = new Dictionary<string, Entity>();
    }

    public bool TryAddEntity(Entity entity)
    {
        if (!_entityPointers.TryAdd(entity.Name, entity))
            return false;
        
        _entities.Add(entity);
        
        if (_hasInitialized)
            entity.Initialize();

        return true;
    }

    public void AddEntity(Entity entity)
    {
        if (!TryAddEntity(entity))
            throw new Exception($"Entity with name \"{entity.Name}\" has already been added to the scene.");
        
        if (_hasInitialized)
            entity.Initialize();
    }

    public virtual void Initialize()
    {
        if (_hasInitialized)
            return;

        _hasInitialized = true;
        
        foreach (Entity entity in _entities)
            entity.Initialize();
    }

    public virtual void Update(float dt)
    {
        foreach (Entity entity in _entities)
            entity.Update(dt);
    }

    public virtual void Draw()
    {
        foreach (Entity entity in _entities)
            entity.Draw();
    }

    public virtual void Dispose()
    {
        foreach (Entity entity in _entities)
            entity.Dispose();
    }
}
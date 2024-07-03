using System;
using System.Collections.Generic;
using Euphoria.Core;
using Euphoria.Engine.Entities;
using Euphoria.Math;
using Euphoria.Render;
using Euphoria.Render.Renderers.Structs;

namespace Euphoria.Engine.Scenes;

public class Scene : IDisposable
{
    // Use that Entities are heap allocated to our advantage.
    // The List is what is iterated through, as its iteration performance is fast.
    // The dictionary is what is used for lookup.
    private List<Entity> _entities;
    private Dictionary<string, Entity> _entityPointers;
    private bool _hasInitialized;
    
    public Camera Camera
    {
        get
        {
            if (!TryGetEntity("Camera", out Camera camera))
                throw new Exception("A camera was not found in the scene. Scenes must have at least one camera.");

            return camera;
        }
    }

    public Scene()
    {
        _entities = new List<Entity>();
        _entityPointers = new Dictionary<string, Entity>();

        AddEntity(new Camera("Camera", new Transform(), 75));
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

    public bool TryGetEntity(string name, out Entity entity)
    {
        return _entityPointers.TryGetValue(name, out entity);
    }

    public bool TryGetEntity<T>(string name, out T entity) where T : Entity
    {
        entity = null;
        
        if (!TryGetEntity(name, out Entity ent))
            return false;

        entity = (T) ent;
        return true;
    }

    public Entity GetEntity(string name)
    {
        if (!TryGetEntity(name, out Entity entity))
            throw new Exception($"Entity with name \"{name}\" was not found in the scene.");

        return entity;
    }

    public T GetEntity<T>(string name) where T : Entity
    {
        return (T) GetEntity(name);
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
        
        Camera camera = Camera;
        Graphics.Renderer3D.Camera = new CameraInfo(camera.ProjectionMatrix, camera.ViewMatrix);
    }

    public virtual void Dispose()
    {
        foreach (Entity entity in _entities)
            entity.Dispose();
    }
}
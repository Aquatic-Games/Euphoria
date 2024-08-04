using System;
using System.Collections.Generic;
using Euphoria.Core;
using Euphoria.Engine.Entities;
using Euphoria.Render;
using Euphoria.Render.Renderers.Structs;

namespace Euphoria.Engine.Scenes;

public class Scene : IDisposable
{
    private Entity[] _entities;
    private Dictionary<string, int> _entityPointers;
    private int _numEntities;
    
    private bool _hasInitialized;

    // TODO: A better way to do this please! I don't like this much.
    public readonly Dictionary<ulong, Entity> BodyIdToEntity;

    public int NumEntities => _numEntities;
    
    public Camera Camera
    {
        get
        {
            if (!TryGetEntity("Camera", out Camera camera))
                throw new Exception("A camera was not found in the scene. Scenes must have at least one camera.");

            return camera;
        }
    }

    protected Scene(int entityAlloc = 16)
    {
        _entities = new Entity[entityAlloc];
        _entityPointers = new Dictionary<string, int>();
        BodyIdToEntity = new Dictionary<ulong, Entity>();

        AddEntity(new Camera("Camera", new Transform(), 75));
    }

    public bool TryAddEntity(Entity entity)
    {
        if (_entityPointers.ContainsKey(entity.Name))
            return false;

        int index = _numEntities++;
        if (index >= _entities.Length)
        {
            int newLength = _entities.Length << 1;
            Logger.Trace($"Resizing entities array to size {newLength}.");
            Array.Resize(ref _entities, _entities.Length << 1);
        }

        _entities[index] = entity;
        _entityPointers.Add(entity.Name, index);
        
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
        entity = null;
        
        if (!_entityPointers.TryGetValue(name, out int index))
            return false;

        entity = _entities[index];
        return true;
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
        
        for (int i = 0; i < _numEntities; i++)
            _entities[i].Initialize();
    }

    public virtual void Tick(float dt)
    {
        for (int i = 0; i < _numEntities; i++)
            _entities[i].Tick(dt);
    }

    public virtual void Update(float dt)
    {
        for (int i = 0; i < _numEntities; i++)
            _entities[i].Update(dt);
    }

    public virtual void Draw()
    {
        for (int i = 0; i < _numEntities; i++)
            _entities[i].Draw();
        
        Camera camera = Camera;
        Graphics.Renderer3D.Camera = new CameraInfo(camera.ProjectionMatrix, camera.ViewMatrix, camera.Transform.Position);
    }

    public virtual void Dispose()
    {
        for (int i = 0; i < _numEntities; i++)
            _entities[i].Dispose();
    }
}
using System;
using System.Collections.Generic;
using u4.Core;
using u4.Engine.Entities;

namespace u4.Engine.Scenes;

public abstract class Scene : IDisposable
{
    private FastList<Entity> _entities;
    private Dictionary<string, uint> _entityPointers;

    public Scene()
    {
        _entities = new FastList<Entity>();
        _entityPointers = new Dictionary<string, uint>();
    }
    
    public virtual void Dispose() { }
}
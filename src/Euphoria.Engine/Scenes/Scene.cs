using System;
using System.Collections.Generic;
using Euphoria.Core;
using Euphoria.Engine.Entities;

namespace Euphoria.Engine.Scenes;

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
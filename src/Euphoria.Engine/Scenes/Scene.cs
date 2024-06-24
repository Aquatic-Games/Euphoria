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

    public Scene()
    {
        _entities = new List<Entity>();
        _entityPointers = new Dictionary<string, Entity>();
    }
    
    
    
    public virtual void Dispose() { }
}
using System.Collections.Generic;

namespace Euphoria.Core;

public class ItemIdCollection<T>
{
    private ulong _currentId;

    public readonly Dictionary<ulong, T> Items;

    public ulong NextId => _currentId + 1;

    public T this[ulong id]
    {
        get => Items[id];
        set => Items[id] = value;
    }

    public ItemIdCollection()
    {
        _currentId = 0;
        Items = new Dictionary<ulong, T>();
    }

    public ulong AddItem(T item)
    {
        ulong id = ++_currentId;
        Items.Add(id, item);
        return id;
    }

    public void RemoveItem(ulong id)
    {
        Items.Remove(id);
    }
}
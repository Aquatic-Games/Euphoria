using System.Collections.Generic;

namespace Euphoria.Engine;

public static class Input
{
    private static HashSet<Key> _keysDown;
    private static HashSet<Key> _frameKeys;

    static Input()
    {
        _keysDown = new HashSet<Key>();
        _frameKeys = new HashSet<Key>();
    }

    public static bool IsKeyDown(Key key) => _keysDown.Contains(key);

    public static bool IsKeyPressed(Key key) => _frameKeys.Contains(key);
    
    internal static void Initialize(Window window)
    {
        window.KeyDown += OnKeyDown;
        window.KeyUp += OnKeyUp;
    }

    internal static void Update()
    {
        _frameKeys.Clear();
    }

    private static void OnKeyDown(Key key)
    {
        _keysDown.Add(key);
        _frameKeys.Add(key);
    }
    
    private static void OnKeyUp(Key key)
    {
        _keysDown.Remove(key);
        _frameKeys.Remove(key);
    }
}
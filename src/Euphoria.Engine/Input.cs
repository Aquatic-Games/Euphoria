using System.Collections.Generic;
using System.Numerics;

namespace Euphoria.Engine;

public static class Input
{
    private static HashSet<Key> _keysDown;
    private static HashSet<Key> _frameKeys;

    private static HashSet<MouseButton> _buttonsDown;
    private static HashSet<MouseButton> _frameButtons;

    private static Vector2 _mousePosition;
    private static Vector2 _mouseDelta;

    static Input()
    {
        _keysDown = new HashSet<Key>();
        _frameKeys = new HashSet<Key>();

        _buttonsDown = new HashSet<MouseButton>();
        _frameButtons = new HashSet<MouseButton>();
    }

    public static Vector2 MousePosition => _mousePosition;

    public static Vector2 MouseDelta => _mouseDelta;

    public static bool IsKeyDown(Key key) => _keysDown.Contains(key);

    public static bool IsKeyPressed(Key key) => _frameKeys.Contains(key);

    public static bool IsMouseButtonDown(MouseButton button) => _buttonsDown.Contains(button);

    public static bool IsMouseButtonPressed(MouseButton button) => _frameButtons.Contains(button);
    
    internal static void Initialize(Window window)
    {
        window.KeyDown += OnKeyDown;
        window.KeyUp += OnKeyUp;
        
        window.MouseButtonDown += OnMouseButtonDown;
        window.MouseButtonUp += OnMouseButtonUp;
        
        window.MouseMove += OnMouseMove;
    }

    internal static void Update()
    {
        _frameKeys.Clear();
        _frameButtons.Clear();
        
        _mouseDelta = Vector2.Zero;
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
    
    private static void OnMouseButtonDown(MouseButton button)
    {
        _buttonsDown.Add(button);
        _frameButtons.Add(button);
    }
    
    private static void OnMouseMove(Vector2 position, Vector2 delta)
    {
        _mousePosition = position;
        _mouseDelta += delta;
    }

    private static void OnMouseButtonUp(MouseButton button)
    {
        _buttonsDown.Remove(button);
        _frameButtons.Remove(button);
    }
}
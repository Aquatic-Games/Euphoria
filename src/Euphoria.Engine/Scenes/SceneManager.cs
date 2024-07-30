using System;

namespace Euphoria.Engine.Scenes;

public static class SceneManager
{
    private static Scene _activeScene;
    private static Scene _sceneToSwitch;

    /// <summary>
    /// The currently active scene. Setting this value will NOT unload the previous scene.
    /// </summary>
    public static Scene ActiveScene
    {
        get => _activeScene;
        set => _activeScene = value;
    }

    /// <summary>
    /// Loads and immediately switches the current scene, unloading the previous scene.
    /// </summary>
    /// <param name="scene">The scene to load and switch to.</param>
    public static void LoadAndSwitchScene(Scene scene)
    {
        _sceneToSwitch = scene;
    }

    internal static void Initialize(Scene scene)
    {
        _activeScene = scene;
        _activeScene.Initialize();
    }

    internal static void Tick(float dt)
    {
        _activeScene.Tick(dt);
    }

    internal static void Update(float dt)
    {
        if (_sceneToSwitch != null)
        {
            _activeScene.Dispose();
            _activeScene = null;
            GC.Collect();
            _activeScene = _sceneToSwitch;
            _sceneToSwitch = null;
            _activeScene.Initialize();
        }
        
        _activeScene.Update(dt);
    }

    internal static void Draw()
    {
        _activeScene.Draw();
    }

    internal static void Unload()
    {
        _activeScene.Dispose();
    }
}
using System;
using Euphoria.Engine;
using Euphoria.Engine.Configs;
using Euphoria.Engine.Debugging;
using Euphoria.Engine.InputSystem;
using Euphoria.Engine.InputSystem.Bindings;
using Euphoria.Engine.Scenes;
using Euphoria.Render;
using Euphoria.Render.Renderers;
using ImGuiNET;

namespace Tests.Engine;

public class TestApp : Application
{
    public const string FileBase = "C:/Users/ollie";

    private float _dtAccumulator;
    private int _tickAccumulator;
    
    public override void Initialize(Scene initialScene)
    {
        ActionSet uiActionSet = new ActionSet("UI", CursorMode.Visible, false);
        uiActionSet.Actions.Add("ExitMenu", new InputAction(new KeyBinding(Key.Escape)));
        Input.AddActionSet("UI", uiActionSet);
        
        ActionSet mainActionSet = new ActionSet("Main", CursorMode.Locked);
        mainActionSet.Actions.Add("Look", new InputAction(new MouseMoveBinding(0.5f), new Binding2D<KeyBinding>(new KeyBinding(Key.Up), new KeyBinding(Key.Down), new KeyBinding(Key.Left), new KeyBinding(Key.Right))));
        mainActionSet.Actions.Add("Move", new InputAction(new Binding2D<KeyBinding>(new KeyBinding(Key.W), new KeyBinding(Key.S), new KeyBinding(Key.A), new KeyBinding(Key.D))));
        mainActionSet.Actions.Add("Jump", new InputAction(new KeyBinding(Key.Space)));
        mainActionSet.Actions.Add("Crouch", new InputAction(new KeyBinding(Key.LeftControl)));
        mainActionSet.Actions.Add("Interact", new InputAction(new KeyBinding(Key.E)));
        mainActionSet.Actions.Add("Exit", new InputAction(new KeyBinding(Key.Escape)));
        Input.AddActionSet("Main", mainActionSet);
        
        if (EuphoriaConfig.CurrentConfig != null && EuphoriaConfig.CurrentConfig.Input.HasValue)
            Input.LoadInputConfig(EuphoriaConfig.CurrentConfig.Input.Value);
        Input.SetActiveActionSet(mainActionSet);
        
        base.Initialize(initialScene);

        //Graphics.VSyncMode = VSyncMode.Off;
        
        ImGuiRenderer imGui = Graphics.ImGuiRenderer;
        imGui.AddFont($"{FileBase}/Downloads/Russo_One/RussoOne-Regular.ttf", 14, "RussoOne");

        //EuphoriaDebug.IsOpen = true;
    }

    public override void Tick(float dt)
    {
        /*_dtAccumulator += dt;
        _tickAccumulator++;

        if (_dtAccumulator >= 1)
        {
            _dtAccumulator -= 1;
            _tickAccumulator = 0;
        }

        Console.WriteLine($"Second {_dtAccumulator} Tick {_tickAccumulator}");*/
        
        base.Tick(dt);
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        
        if (Input.GetActionSet("Main").GetAction("Exit").IsPressed)
            App.Close();

        if (Input.GetActionSet("UI").GetAction("ExitMenu").IsPressed)
            EuphoriaDebug.IsOpen = false;

        if (Input.UIWantsFocus)
            Input.SetActiveActionSet(Input.GetActionSet("UI"));
        else
            Input.SetActiveActionSet(Input.GetActionSet("Main"));
    }

    public override void Dispose()
    {
        EuphoriaConfig config = EuphoriaConfig.CreateFromCurrentSettings();
        config.Save("Config.cfg");
    }
}
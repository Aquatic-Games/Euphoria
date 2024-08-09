using System;
using System.Collections.Generic;
using Euphoria.Engine.InputSystem.Actions;

namespace Euphoria.Engine.InputSystem;

public class InputScene
{
    public readonly string FriendlyName;

    public readonly CursorMode? CursorMode;

    public readonly Dictionary<string, IInputAction> Actions;

    public InputScene(string friendlyName)
    {
        FriendlyName = friendlyName;
        Actions = new Dictionary<string, IInputAction>();
    }

    public T GetAction<T>(string name) where T : IInputAction
        => (T) Actions[name];

    public void Update()
    {
        foreach ((_, IInputAction action) in Actions)
            action.Update();
    }
}
using System;
using System.Collections.Generic;
using Euphoria.Engine.InputSystem.Actions;

namespace Euphoria.Engine.InputSystem;

public class ActionSet
{
    public readonly string FriendlyName;

    public readonly CursorMode? CursorMode;

    public readonly Dictionary<string, InputAction> Actions;

    public ActionSet(string friendlyName, CursorMode? cursorMode = null)
    {
        FriendlyName = friendlyName;
        Actions = new Dictionary<string, InputAction>();
        CursorMode = cursorMode;
    }

    public T GetAction<T>(string name) where T : InputAction
        => (T) Actions[name];

    public virtual void Update()
    {
        foreach ((_, InputAction action) in Actions)
            action.Update();
    }
}
using System;
using System.Collections.Generic;
using Euphoria.Engine.InputSystem.Actions;

namespace Euphoria.Engine.InputSystem;

public class ActionSet
{
    public readonly string FriendlyName;

    public readonly CursorMode? CursorMode;

    public readonly Dictionary<string, IInputAction> Actions;

    public ActionSet(string friendlyName, CursorMode? cursorMode = null)
    {
        FriendlyName = friendlyName;
        Actions = new Dictionary<string, IInputAction>();
        CursorMode = cursorMode;
    }

    public T GetAction<T>(string name) where T : IInputAction
        => (T) Actions[name];

    public virtual void Update()
    {
        foreach ((_, IInputAction action) in Actions)
            action.Update();
    }
}
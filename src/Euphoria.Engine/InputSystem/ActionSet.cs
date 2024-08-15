using System;
using System.Collections.Generic;
using Euphoria.Engine.InputSystem.Actions;

namespace Euphoria.Engine.InputSystem;

public class ActionSet
{
    public readonly string FriendlyName;
    
    public readonly CursorMode? CursorMode;

    public readonly bool Save;

    public readonly Dictionary<string, InputAction> Actions;

    public ActionSet(string friendlyName, CursorMode? cursorMode = null, bool save = true)
    {
        FriendlyName = friendlyName;
        Actions = new Dictionary<string, InputAction>();
        CursorMode = cursorMode;
        Save = save;
    }

    public InputAction GetAction(string name)
        => Actions[name];

    public virtual void Update()
    {
        foreach ((_, InputAction action) in Actions)
            action.Update();
    }
}
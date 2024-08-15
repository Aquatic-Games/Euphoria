using System;
using System.Collections.Generic;
using Euphoria.Core;
using Euphoria.Engine.InputSystem;
using Euphoria.Parsers;

namespace Euphoria.Engine.Configs;

public struct InputConfig : ISerializableConfig<InputConfig>
{
    public Dictionary<string, InputAction> FlattenedActions;

    public InputConfig(Dictionary<string, InputAction> flattenedActions)
    {
        FlattenedActions = flattenedActions;
    }

    public void WriteIni(Ini ini)
    {
        Ini.Group group = new Ini.Group("Input");
        
        foreach ((string name, InputAction action) in FlattenedActions)
            group.AddItem(name, new Ini.Item(Ini.ItemType.String, action.ToString()));
        
        ini.AddGroup(group);
    }

    public static bool TryFromIni(Ini ini, out InputConfig config)
    {
        Ini.Group group = ini.Groups["Input"];
        Dictionary<string, InputAction> flattenedActions = new Dictionary<string, InputAction>();

        foreach ((string name, Ini.Item item) in group.Items)
        {
            try
            {
                flattenedActions.Add(name, InputAction.FromString((string) item.Value));
            }
            catch (Exception e)
            {
                Logger.Error($"Parsing input action failed: {e.Message}");
            }
        }

        config = new InputConfig(flattenedActions);
        return true;
    }
}
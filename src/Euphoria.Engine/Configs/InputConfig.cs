using System.Collections.Generic;
using Euphoria.Engine.InputSystem.Actions;
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
        throw new System.NotImplementedException();
    }
}
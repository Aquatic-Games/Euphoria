using System;
using System.Collections.Generic;
using Euphoria.Parsers;
using grabs.Graphics;

namespace Euphoria.Engine.Configs;

public struct GraphicsConfig : ISerializableConfig<GraphicsConfig>
{
    public GraphicsApi? Api;

    public int? Adapter;

    public GraphicsConfig(GraphicsApi? api, int? adapter)
    {
        Api = api;
        Adapter = adapter;
    }

    public void WriteIni(Ini ini)
    {
        ini.Groups.Add("Graphics", new Ini.Group("Graphics", new Dictionary<string, Ini.Item>()
        {
            ["Api"] = new Ini.Item(Ini.ItemType.String, Api.ToString()),
            ["Adapter"] = new Ini.Item(Ini.ItemType.Number, Adapter)
        }));
    }

    public static bool TryFromIni(Ini ini, out GraphicsConfig config)
    {
        config = default;
        
        if (!ini.TryGetGroup("Graphics", out Ini.Group group))
            return false;
        
        GraphicsApi? api = group.GetItemOrDefault<GraphicsApi?>("Api");
        int? adapter = group.GetItemOrDefault<int?>("Adapter");

        config = new GraphicsConfig(api, adapter);
        return true;
    }
}
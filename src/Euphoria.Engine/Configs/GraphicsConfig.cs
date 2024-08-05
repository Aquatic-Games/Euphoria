using System;
using System.Collections.Generic;
using Euphoria.Parsers;
using grabs.Graphics;

namespace Euphoria.Engine.Configs;

public struct GraphicsConfig : ISerializableConfig<GraphicsConfig>
{
    public GraphicsApi Api;

    public int Adapter;

    public GraphicsConfig(GraphicsApi api, int adapter)
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

    public static GraphicsConfig FromIni(Ini ini)
    {
        Ini.Group group = ini.Groups["Graphics"];
        GraphicsApi api = Enum.Parse<GraphicsApi>((string) group.Items["Api"].Value);
        int adapter = (int) (double) group.Items["Adapter"].Value;

        return new GraphicsConfig(api, adapter);
    }
}
using Euphoria.Render;
using grabs.Graphics;

namespace Euphoria.Engine;

public struct GraphicsInfo
{
    public GraphicsApi Api;

    public int AdapterIndex;

    public GraphicsSettings Settings;

    public GraphicsInfo(GraphicsApi api, int adapterIndex, GraphicsSettings settings)
    {
        Api = api;
        AdapterIndex = adapterIndex;
        Settings = settings;
    }

    public static GraphicsInfo Default => new()
    {
        Api = App.PickBestGraphicsApi(),
        AdapterIndex = 0,
        Settings = GraphicsSettings.Default
    };

    public static GraphicsInfo DefaultWithApi(GraphicsApi api)
        => Default with { Api = api };
}
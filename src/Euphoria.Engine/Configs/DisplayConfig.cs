using System;
using System.Collections.Generic;
using Euphoria.Math;
using Euphoria.Parsers;
using Euphoria.Render;

namespace Euphoria.Engine.Configs;

public struct DisplayConfig : ISerializableConfig<DisplayConfig>
{
    public Size<int> Size;

    public FullscreenMode FullscreenMode;

    public VSyncMode VSync;

    public DisplayConfig(Size<int> size, FullscreenMode fullscreenMode, VSyncMode vSync)
    {
        Size = size;
        FullscreenMode = fullscreenMode;
        VSync = vSync;
    }

    public void WriteIni(Ini ini)
    {
        ini.Groups.Add("Display", new Ini.Group("Display", new Dictionary<string, Ini.Item>()
        {
            ["Width"] = new Ini.Item(Ini.ItemType.Number, Size.Width),
            ["Height"] = new Ini.Item(Ini.ItemType.Number, Size.Height),
            ["Fullscreen"] = new Ini.Item(Ini.ItemType.String, FullscreenMode.ToString()),
            ["VSync"] = new Ini.Item(Ini.ItemType.String, VSync.ToString())
        }));
    }

    public static DisplayConfig FromIni(Ini ini)
    {
        Ini.Group group = ini.Groups["Display"];

        Size<int> size = new Size<int>()
        {
            Width = (int) (double) group.Items["Width"].Value,
            Height = (int) (double) group.Items["Height"].Value
        };

        FullscreenMode mode = Enum.Parse<FullscreenMode>((string) group.Items["Fullscreen"].Value);
        VSyncMode vsync = Enum.Parse<VSyncMode>((string) group.Items["VSync"].Value);

        return new DisplayConfig(size, mode, vsync);
    }
}
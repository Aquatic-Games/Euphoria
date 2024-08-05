using System;
using System.Collections.Generic;
using Euphoria.Math;
using Euphoria.Parsers;
using Euphoria.Render;

namespace Euphoria.Engine.Configs;

public struct DisplayConfig : ISerializableConfig<DisplayConfig>
{
    public Size<int>? Size;

    public FullscreenMode? FullscreenMode;

    public VSyncMode? VSync;

    public DisplayConfig(Size<int>? size, FullscreenMode? fullscreenMode, VSyncMode? vSync)
    {
        Size = size;
        FullscreenMode = fullscreenMode;
        VSync = vSync;
    }

    public void WriteIni(Ini ini)
    {
        ini.Groups.Add("Display", new Ini.Group("Display", new Dictionary<string, Ini.Item>()
        {
            ["Width"] = new Ini.Item(Ini.ItemType.Number, Size?.Width),
            ["Height"] = new Ini.Item(Ini.ItemType.Number, Size?.Height),
            ["Fullscreen"] = new Ini.Item(Ini.ItemType.String, FullscreenMode.ToString()),
            ["VSync"] = new Ini.Item(Ini.ItemType.String, VSync.ToString())
        }));
    }

    public static bool TryFromIni(Ini ini, out DisplayConfig config)
    {
        config = default;
        
        if (!ini.TryGetGroup("Display", out Ini.Group group))
            return false;

        Size<int>? size = null;
        if (group.TryGetItem("Width", out int width) && group.TryGetItem("Height", out int height))
            size = new Size<int>(width, height);

        FullscreenMode? mode = group.GetItemOrDefault<FullscreenMode?>("Fullscreen");
        VSyncMode? vsync = group.GetItemOrDefault<VSyncMode?>("VSync");

        config = new DisplayConfig(size, mode, vsync);
        return true;
    }
}
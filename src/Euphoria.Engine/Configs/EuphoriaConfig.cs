using System;
using System.IO;
using Euphoria.Core;
using Euphoria.Parsers;
using Euphoria.Render;

namespace Euphoria.Engine.Configs;

public class EuphoriaConfig : IConfig<EuphoriaConfig>
{
    public DisplayConfig Display;

    public GraphicsConfig Graphics;

    public EuphoriaConfig(DisplayConfig display, GraphicsConfig graphics)
    {
        Display = display;
        Graphics = graphics;
    }

    public static EuphoriaConfig CreateFromCurrentSettings()
    {
        DisplayConfig display = new DisplayConfig()
        {
            Size = Window.SizeInPixels,
            FullscreenMode = Window.FullscreenMode,
            VSync = Render.Graphics.VSyncMode
        };

        GraphicsConfig graphics = new GraphicsConfig()
        {
            Api = Render.Graphics.Api,
            Adapter = (int) Render.Graphics.Adapter.Index
        };

        return new EuphoriaConfig(display, graphics);
    }

    public virtual void WriteIni(Ini ini)
    {
        Display.WriteIni(ini);
        Graphics.WriteIni(ini);
    }

    public void Save(string path)
    {
        Logger.Debug($"Saving config to {path}");
        
        Ini ini = new Ini();
        WriteIni(ini);
        
        File.WriteAllText(path, ini.Serialize());
    }

    public static EuphoriaConfig CurrentConfig { get; set; }

    public static EuphoriaConfig FromIni(Ini ini)
    {
        DisplayConfig display = DisplayConfig.FromIni(ini);
        GraphicsConfig graphics = GraphicsConfig.FromIni(ini);

        return new EuphoriaConfig(display, graphics);
    }

    public static bool TryLoadFromFile(string path, out EuphoriaConfig config)
    {
        config = default;
        
        if (!File.Exists(path))
            return false;

        string text = File.ReadAllText(path);

        Ini ini;
        try
        {
            ini = new Ini(text);
        }
        catch (Exception)
        {
            return false;
        }

        config = FromIni(ini);
        return true;
    }
}
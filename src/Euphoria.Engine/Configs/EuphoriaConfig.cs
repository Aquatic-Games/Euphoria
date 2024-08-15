using System;
using System.Collections.Generic;
using System.IO;
using Euphoria.Core;
using Euphoria.Engine.InputSystem;
using Euphoria.Engine.InputSystem.Actions;
using Euphoria.Parsers;
using Euphoria.Render;

namespace Euphoria.Engine.Configs;

public class EuphoriaConfig : IConfig<EuphoriaConfig>
{
    public DisplayConfig? Display;

    public GraphicsConfig? Graphics;

    public InputConfig? Input;

    public EuphoriaConfig(DisplayConfig? display, GraphicsConfig? graphics, InputConfig? input)
    {
        Display = display;
        Graphics = graphics;
        Input = input;
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

        Dictionary<string, InputAction> flattenedActions = new Dictionary<string, InputAction>();
        foreach ((string setName, ActionSet set) in InputSystem.Input.GetAllActionSets())
        {
            if (!set.Save)
                continue;
            
            foreach ((string actionName, InputAction action) in set.Actions)
            {
                flattenedActions.Add($"{setName}.{actionName}", action);
            }
        }

        InputConfig input = new InputConfig(flattenedActions);
        
        return new EuphoriaConfig(display, graphics, input);
    }

    public virtual void WriteIni(Ini ini)
    {
        Display?.WriteIni(ini);
        Graphics?.WriteIni(ini);
        Input?.WriteIni(ini);
    }

    public void Save(string path)
    {
        Logger.Debug($"Saving config to {path}");
        
        Ini ini = new Ini();
        WriteIni(ini);
        
        File.WriteAllText(path, ini.Serialize());
    }

    public static EuphoriaConfig CurrentConfig { get; set; }

    public static bool TryFromIni(Ini ini, out EuphoriaConfig config)
    {
        config = new EuphoriaConfig(null, null, null);

        if (DisplayConfig.TryFromIni(ini, out DisplayConfig display))
            config.Display = display;

        if (GraphicsConfig.TryFromIni(ini, out GraphicsConfig graphics))
            config.Graphics = graphics;

        if (InputConfig.TryFromIni(ini, out InputConfig input))
            config.Input = input;
        
        return true;
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

        return TryFromIni(ini, out config);
    }
}
using System;
using System.Collections.Generic;
using Euphoria.Engine.InputSystem;
using ImGuiNET;

namespace Euphoria.Engine.Debugging;

public static class EuphoriaDebug
{
    private static bool _open;
    private static bool _pinned;

    public static Dictionary<Type, IDebugTab> Tabs;

    public static bool IsOpen => _open;

    public static bool IsPinned => _pinned;

    static EuphoriaDebug()
    {
        _open = false;
        _pinned = false;
        //Tabs = [new DebugConsole(), new StatsTab(), new RendererTab()];
        Tabs = new Dictionary<Type, IDebugTab>();
        AddTab(new DebugConsole());
        AddTab(new StatsTab());
        AddTab(new RendererTab());
    }

    public static void Open()
    {
        _open = true;
        _pinned = false;
    }

    public static void Close()
    {
        _open = false;
        _pinned = false;
    }

    public static void Pin()
    {
        _open = true;
        _pinned = true;
    }

    public static T GetTab<T>() where T : IDebugTab
    {
        return (T) Tabs[typeof(T)];
    }

    public static void AddTab(IDebugTab tab)
    {
        Tabs.Add(tab.GetType(), tab);
    }

    internal static void Update()
    {
        if (Input.IsKeyDown(Key.LeftShift) && Input.IsKeyPressed(Key.F12))
            Open();
        
        if (!_open)
            return;

        if (!_pinned)
            Input.UIWantsFocus = true;

        if (ImGui.Begin("Debug", ref _open, ImGuiWindowFlags.MenuBar | (_pinned ? ImGuiWindowFlags.NoInputs : 0)))
        {
            if (ImGui.BeginMenuBar())
            {
                if (ImGui.MenuItem("Pin", "", _pinned))
                    Pin();
                
                ImGui.EndMenuBar();
            }
            
            if (ImGui.BeginTabBar("debugTabs"))
            {
                foreach ((_, IDebugTab tab) in Tabs)
                {
                    if (ImGui.BeginTabItem(tab.TabName))
                    {
                        tab.Update();
                        ImGui.EndTabItem();
                    }
                }
                
                ImGui.EndTabBar();
            }

            ImGui.End();
        }
    }

    internal static void Draw()
    {
        foreach ((_, IDebugTab tab) in Tabs)
            tab.Draw();
    }
}
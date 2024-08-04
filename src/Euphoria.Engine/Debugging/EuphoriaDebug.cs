using System;
using System.Collections.Generic;
using Euphoria.Engine.InputSystem;
using ImGuiNET;

namespace Euphoria.Engine.Debugging;

public static class EuphoriaDebug
{
    public static bool IsOpen;

    public static Dictionary<Type, IDebugTab> Tabs;

    static EuphoriaDebug()
    {
        IsOpen = false;
        //Tabs = [new DebugConsole(), new StatsTab(), new RendererTab()];
        Tabs = new Dictionary<Type, IDebugTab>();
        AddTab(new DebugConsole());
        AddTab(new StatsTab());
        AddTab(new RendererTab());
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
        {
            IsOpen = !IsOpen;
        }
        
        if (!IsOpen)
            return;

        if (ImGui.Begin("Debug", ref IsOpen))
        {
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
using System.Collections.Generic;
using ImGuiNET;

namespace Euphoria.Engine.Debugging;

public static class EuphoriaDebug
{
    public static bool IsOpen;

    public static List<IDebugTab> Tabs;

    static EuphoriaDebug()
    {
        IsOpen = false;
        Tabs = [new DebugConsole(), new StatsTab(), new RendererTab()];
    }

    internal static void Update()
    {
        if (!IsOpen)
            return;

        if (ImGui.Begin("Debug"))
        {
            if (ImGui.BeginTabBar("debugTabs"))
            {
                foreach (IDebugTab tab in Tabs)
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
        foreach (IDebugTab tab in Tabs)
            tab.Draw();
    }
}
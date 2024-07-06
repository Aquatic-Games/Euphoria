using ImGuiNET;

namespace Euphoria.Engine.Debugging;

public class StatsTab : IDebugTab
{
    public string TabName => "Stats";
    
    public void Update()
    {
        ImGui.Text($"App name: {App.Name}");
        ImGui.Text($"App version: {App.Version}");
        
        ImGui.Separator();
        
        ImGui.Text($"Frame: {Metrics.TotalFrames}");
        ImGui.Text($"FPS: {Metrics.FramesPerSecond} (dt: {double.Round(Metrics.TimeSinceLastFrame * 1000, 1)})");
    }
}
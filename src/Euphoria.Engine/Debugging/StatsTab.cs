using System;
using System.Numerics;
using System.Text;
using Euphoria.Engine.Scenes;
using Euphoria.Math;
using Euphoria.Render;
using Euphoria.Render.Text;
using grabs.Graphics;
using ImGuiNET;

namespace Euphoria.Engine.Debugging;

public class StatsTab : IDebugTab
{
    private StringBuilder _debugText;
    
    public int DebugVerbosity;
    
    public string TabName => "Stats";

    public StatsTab()
    {
        _debugText = new StringBuilder();
        DebugVerbosity = 0;
    }
    
    public void Update()
    {
        ImGui.Text($"App name: {App.Name}");
        ImGui.Text($"App version: {App.Version}");

        ImGui.InputInt("Debug Verbosity", ref DebugVerbosity);
        DebugVerbosity = int.Clamp(DebugVerbosity, 0, 3);
        
        ImGui.Separator();

        if (ImGui.CollapsingHeader("Performance"))
        {
            ImGui.Text($"Frame: {Metrics.TotalFrames}");
            ImGui.Text($"FPS: {Metrics.FramesPerSecond} (dt: {double.Round(Metrics.TimeSinceLastFrame * 1000, 1)})");
        }

        if (ImGui.CollapsingHeader("Scene"))
        {
            Scene activeScene = SceneManager.ActiveScene;
            ImGui.Text($"Entities: {activeScene.NumEntities}");
        }

        if (ImGui.CollapsingHeader("Renderer"))
        {
            Adapter adapter = Graphics.Adapter;
            ImGui.Text($"Adapter:");
            ImGui.Text($"  Name: {adapter.Name}");
            ImGui.Text($"  Memory: {adapter.DedicatedMemory / 1024 / 1024} MB");
            ImGui.Text($"  Index: {adapter.Index}");
        }
    }

    public void Draw()
    {
        if (DebugVerbosity == 0)
            return;
        
        _debugText.Clear();
        Scene activeScene = SceneManager.ActiveScene;
        
        _debugText.AppendLine($"Frame: {Metrics.TotalFrames}");
        _debugText.AppendLine(
            $"FPS: {Metrics.FramesPerSecond} (dt: {double.Round(Metrics.TimeSinceLastFrame * 1000, 1)})");

        if (DebugVerbosity >= 2)
        {
            _debugText.AppendLine($"Entities: {activeScene.NumEntities}");
        }

        if (DebugVerbosity >= 3)
        {
            _debugText.AppendLine($"{Graphics.Adapter}");
        }

        Font.Roboto.Draw(Graphics.TextureBatcher, new Vector2(5), _debugText.ToString(), 12, Color.White);
    }
}
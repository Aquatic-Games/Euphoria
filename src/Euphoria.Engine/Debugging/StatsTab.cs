using System;
using System.Numerics;
using System.Text;
using Euphoria.Engine.Scenes;
using Euphoria.Math;
using Euphoria.Render;
using Euphoria.Render.Text;
using ImGuiNET;

namespace Euphoria.Engine.Debugging;

public class StatsTab : IDebugTab
{
    private StringBuilder _debugText;
    private int _debugVerbosity;
    
    public string TabName => "Stats";

    public StatsTab()
    {
        _debugText = new StringBuilder();
        _debugVerbosity = 0;
    }
    
    public void Update()
    {
        ImGui.Text($"App name: {App.Name}");
        ImGui.Text($"App version: {App.Version}");

        ImGui.InputInt("Debug Verbosity", ref _debugVerbosity);
        _debugVerbosity = int.Clamp(_debugVerbosity, 0, 2);
        
        ImGui.Separator();
        
        ImGui.Text($"Frame: {Metrics.TotalFrames}");
        ImGui.Text($"FPS: {Metrics.FramesPerSecond} (dt: {double.Round(Metrics.TimeSinceLastFrame * 1000, 1)})");
        
        Scene activeScene = SceneManager.ActiveScene;
        ImGui.Text($"Entities: {activeScene.NumEntities}");
    }

    public void Draw()
    {
        if (_debugVerbosity == 0)
            return;
        
        _debugText.Clear();
        Scene activeScene = SceneManager.ActiveScene;
        
        _debugText.AppendLine($"Frame: {Metrics.TotalFrames}");
        _debugText.AppendLine(
            $"FPS: {Metrics.FramesPerSecond} (dt: {double.Round(Metrics.TimeSinceLastFrame * 1000, 1)})");

        if (_debugVerbosity >= 2)
        {
            _debugText.AppendLine($"Entities: {activeScene.NumEntities}");
        }

        Font.Roboto.Draw(Graphics.TextureBatcher, new Vector2(5), _debugText.ToString(), 12, Color.White);
    }
}
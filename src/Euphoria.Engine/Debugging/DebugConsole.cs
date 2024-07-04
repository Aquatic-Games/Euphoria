using System;
using System.Collections.Generic;
using System.Numerics;
using Euphoria.Core;
using Euphoria.Math;
using ImGuiNET;

namespace Euphoria.Engine.Debugging;

public class DebugConsole : IDebugTab
{
    private Queue<Message> _messages;
    private bool _needsScroll;
    
    public string TabName => "Console";

    public DebugConsole()
    {
        _messages = new Queue<Message>();
        Logger.LogMessage += (type, message) => WriteToConsole(new Message(message));
    }
    
    public void Update()
    {
        float sizeOffset = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();
        
        if (ImGui.BeginChild("ConsoleText", new Vector2(0, -sizeOffset), ImGuiChildFlags.None, ImGuiWindowFlags.HorizontalScrollbar))
        {
            foreach (Message message in _messages)
                ImGui.Text(message.Text);
            
            if (_needsScroll)
            {
                _needsScroll = false;
                ImGui.SetScrollHereY(1.0f);
            }
            
            ImGui.EndChild();
        }

        ImGui.Separator();
        
        string text = "";
        if (ImGui.InputTextWithHint("##text", "Text", ref text, 1000, ImGuiInputTextFlags.EnterReturnsTrue))
            WriteToConsole(new Message(text));
    }

    private void WriteToConsole(Message text)
    {
        _messages.Enqueue(text);
        _needsScroll = true;

        while (_messages.Count > 1000)
            _messages.Dequeue();
    }

    private struct Message
    {
        public string Text;
        public Color? Color;

        public Message(string text, Color? color = null)
        {
            Text = text;
            Color = color;
        }
    }
}
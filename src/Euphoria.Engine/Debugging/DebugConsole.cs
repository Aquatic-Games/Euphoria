using System;
using System.Collections.Generic;
using System.Numerics;
using Euphoria.Core;
using Euphoria.Engine.Debugging.Commands;
using Euphoria.Engine.Debugging.Commands.Builtin;
using Euphoria.Math;
using ImGuiNET;

namespace Euphoria.Engine.Debugging;

public class DebugConsole : IDebugTab
{
    private Queue<Message> _messages;
    private bool _needsScroll;

    public Dictionary<string, ICommand> Commands;
    
    public string TabName => "Console";

    public DebugConsole()
    {
        _messages = new Queue<Message>();
        //Logger.LogMessage += (type, message) => WriteToConsole(new Message(message));
        
        Write("Euphoria debug console. Type \"help\" for a list of commands.", Color.Gray);

        Commands = new Dictionary<string, ICommand>();
        AddCommand(new HelpCommand());
        AddCommand(new DebugDisplayInfoCommand());
        AddCommand(new SetWindowSizeCommand());
    }
    
    public void Update()
    {
        float sizeOffset = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();
        
        if (ImGui.BeginChild("ConsoleText", new Vector2(0, -sizeOffset), ImGuiChildFlags.None, ImGuiWindowFlags.HorizontalScrollbar))
        {
            foreach (Message message in _messages)
            {
                if (message.Color.HasValue)
                {
                    Color color = message.Color.Value;
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(color.R, color.G, color.B, color.A));
                    ImGui.Text(message.Text);
                    ImGui.PopStyleColor();
                }
                else
                    ImGui.Text(message.Text);
            }

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
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                string cmdName = GetCommand(text, out string[] args);
                Write($"# {cmdName}", Color.DarkSalmon);

                if (Commands.TryGetValue(cmdName, out ICommand command))
                {
                    if (TryProcessArguments(command.Arguments, args, out object[] processedArgs, out string error))
                        command.Execute(this, processedArgs);
                    else
                        Write(error, Color.Red);
                }
                else
                    Write("Command not found.", Color.Red);
            }
        }
    }

    public void AddCommand(ICommand command)
    {
        Commands.Add(command.Name, command);
    }

    public void Write(object message, Color? color = null)
    {
        _messages.Enqueue(new Message(message.ToString(), color));
        _needsScroll = true;

        while (_messages.Count > 1000)
            _messages.Dequeue();
    }

    private static string GetCommand(string text, out string[] args)
    {
        string[] splitText = text.Split(' ');
        string cmdName = splitText[0];
        if (splitText.Length == 1)
            args = [];
        else
            args = splitText[1..];

        return cmdName;
    }

    private static bool TryProcessArguments(Argument[] arguments, string[] args, out object[] resultArgs, out string error)
    {
        if (arguments.Length != args.Length)
        {
            error = $"Not enough arguments. Expected {arguments.Length} arguments, found {args.Length}.";
            resultArgs = null;
            return false;
        }
        
        resultArgs = new object[arguments.Length];
        error = null;
        
        for (int i = 0; i < arguments.Length; i++)
        {
            ref Argument argument = ref arguments[i];

            switch (argument.Type)
            {
                case ArgumentType.String:
                    resultArgs[i] = args[i];
                    break;
                
                case ArgumentType.Bool:
                {
                    if (!bool.TryParse(args[i], out bool result))
                    {
                        error = $"Failed to parse argument {i}. Expected boolean.";
                        return false;
                    }

                    resultArgs[i] = result;

                    break;
                }
                
                case ArgumentType.Int:
                {
                    if (!int.TryParse(args[i], out int result))
                    {
                        error = $"Failed to parse argument {i}. Expected integer.";
                        return false;
                    }

                    resultArgs[i] = result;
                    
                    break;
                }

                case ArgumentType.Double:
                {
                    if (!double.TryParse(args[i], out double result))
                    {
                        error = $"Failed to parse argument {i}. Expected number.";
                        return false;
                    }

                    resultArgs[i] = result;
                    
                    break;
                }
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return true;
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
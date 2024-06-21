using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

// ReSharper disable ExplicitCallerInfoArgument

namespace Euphoria.Core;

public static class Logger
{
    public static event OnLogMessage LogMessage = delegate { };
    
    private static StringBuilder _builder = new StringBuilder();
    
    public static void Log(LogType type, string message, [CallerLineNumber] int lineNumber = 0,
        [CallerFilePath] string fileName = "")
    {
        _builder.Clear();

        _builder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff "));

        _builder.Append(type switch
        {
            LogType.Trace => "[Trace] ",
            LogType.Debug => "[Debug] ",
            LogType.Info => "[Info]  ",
            LogType.Warning => "[Warn]  ",
            LogType.Error => "[Error] ",
            LogType.Fatal => "[FATAL] ",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        });

        _builder.Append('(');
        _builder.Append(Path.GetFileName(fileName));
        _builder.Append(':');
        _builder.Append(lineNumber);
        _builder.Append(')');
        _builder.Append(' ');

        _builder.Append(message);
        
        LogMessage.Invoke(type, _builder.ToString());
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Trace(string message, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string fileName = "")
        => Log(LogType.Trace, message, lineNumber, fileName);
    
    public static void Debug(string message, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string fileName = "")
        => Log(LogType.Debug, message, lineNumber, fileName);
    
    public static void Info(string message, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string fileName = "")
        => Log(LogType.Info, message, lineNumber, fileName);
    
    public static void Warn(string message, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string fileName = "")
        => Log(LogType.Warning, message, lineNumber, fileName);
    
    public static void Error(string message, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string fileName = "")
        => Log(LogType.Error, message, lineNumber, fileName);
    
    public static void Fatal(string message, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string fileName = "")
        => Log(LogType.Fatal, message, lineNumber, fileName);

    public static void AttachConsole()
    {
        LogMessage += (type, message) =>
        {
            ConsoleColor currentColor = Console.ForegroundColor;
            
            Console.ForegroundColor = type switch
            {
                LogType.Trace => ConsoleColor.Gray,
                LogType.Debug => ConsoleColor.White,
                LogType.Info => ConsoleColor.Cyan,
                LogType.Warning => ConsoleColor.Yellow,
                LogType.Error => ConsoleColor.Red,
                LogType.Fatal => ConsoleColor.DarkRed,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            Console.WriteLine(message);

            Console.ForegroundColor = currentColor;
        };
    }
    
    public enum LogType
    {
        Trace,
        Debug,
        Info,
        Warning,
        Error,
        Fatal
    }

    public delegate void OnLogMessage(LogType type, string formattedMessage);
}

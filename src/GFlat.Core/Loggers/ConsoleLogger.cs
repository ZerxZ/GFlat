﻿using System.Runtime.InteropServices;
using System.Text;

namespace GFlat.Loggers;

public class ConsoleLogger : ILogger
{
    private const ConsoleColor DefaultColor = ConsoleColor.Gray;

    public static readonly ILogger Default = new ConsoleLogger();
    public static readonly ILogger Ascii   = new ConsoleLogger(false);
    public static readonly ILogger Unicode = new ConsoleLogger(true);
    private static readonly bool ConsoleSupportsColors
        = !(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX));

    private readonly bool                              unicodeSupport;
    private readonly Dictionary<LogKind, ConsoleColor> colorScheme;

    public ConsoleLogger(bool unicodeSupport = false, Dictionary<LogKind, ConsoleColor>? colorScheme = null)
    {
        this.unicodeSupport = unicodeSupport;
        this.colorScheme = colorScheme ?? CreateColorfulScheme();
    }

    public string Id => nameof(ConsoleLogger);

    public void Write(LogKind logKind, string text) => Write(logKind, Console.Write, text);

    public void WriteLine() => Console.WriteLine();

    public void WriteLine(LogKind logKind, string text) => Write(logKind, Console.WriteLine, text);

    public void Flush()
    {
    }

    private void Write(LogKind logKind, Action<string> write, string text)
    {
        if (!unicodeSupport)
            text = text.ToAscii() ?? "";

        if (!ConsoleSupportsColors)
        {
            write(text);
            return;
        }

        var colorBefore = Console.ForegroundColor;

        try
        {
            var color = GetColor(logKind);
            if (color != Console.ForegroundColor && color != Console.BackgroundColor)
                Console.ForegroundColor = color;

            write(text);
        }
        finally
        {
            Console.ForegroundColor = colorBefore;
        }
    }

    private ConsoleColor GetColor(LogKind logKind) =>
        colorScheme.ContainsKey(logKind) ? colorScheme[logKind] : DefaultColor;

    private static Dictionary<LogKind, ConsoleColor> CreateColorfulScheme() =>
        new Dictionary<LogKind, ConsoleColor>
        {
            { LogKind.Default, ConsoleColor.Gray },
            { LogKind.Help, ConsoleColor.DarkGreen },
            { LogKind.Header, ConsoleColor.Magenta },
            { LogKind.Result, ConsoleColor.DarkCyan },
            { LogKind.Statistic, ConsoleColor.Cyan },
            { LogKind.Info, ConsoleColor.DarkYellow },
            { LogKind.Error, ConsoleColor.Red },
            { LogKind.Hint, ConsoleColor.DarkCyan }
        };


    public static Dictionary<LogKind, ConsoleColor> CreateGrayScheme()
    {
        var colorScheme = new Dictionary<LogKind, ConsoleColor>();
        foreach (var logKind in Enum.GetValues(typeof(LogKind)).Cast<LogKind>())
            colorScheme[logKind] = ConsoleColor.Gray;
        return colorScheme;
    }
}
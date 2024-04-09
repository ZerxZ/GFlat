using System.Diagnostics;
using System.Text;
using GFlat.GDExtension;
using GFlat.Loggers;

namespace GFlat.BFlat;

public static class BFlatBuilder
{
    public static readonly string[] tiny      = ["--no-stacktrace-data", "--no-globalization", "--no-exception-messages", "-Os", "--no-pie", "--separate-symbols"];
    public const           string   BFlat     = "bflat";
    public static          bool     InitBFlat = false;
    public static          ILogger  Logger => ConsoleLogger.Unicode;

    private static StringBuilder _sb = new StringBuilder();
    public static bool HasBFlat()
    {
        if (InitBFlat)
        {
            return true;
        }
        try
        {
            var process = Process.Start(GFlat.GetProcessStartInfo(BFlat, "-v"));
            process!.WaitForExit();
        }
        catch (Exception e)
        {
            Logger.WriteLineError("BFlat not found: " + e.Message);
            Logger.WriteLineError("Please install BFlat from https://github.com/bflattened/bflat");
            return false;
        }
        InitBFlat = true;
        return true;
    }
    public static bool Build(string input, string output, Platform platform, Architecture architecture, ReadOnlySpan<string> reference, bool isTiny = false)
    {
        if (!HasBFlat())
        {
            return false;
        }
        _sb.Append($"build  \"{input}\" -o \"{output}\" --target Shared");
        _sb.Append($" --os {platform switch
        {
            Platform.Windows => "windows",
            Platform.Linux   => "linux",
            _                => "linux"
        }}");
        _sb.Append($" --arch {architecture switch
        {
            Architecture.X86_32 => "x86",
            Architecture.X86_64 => "x64",
            Architecture.Arm64  => "arm64",
            _                   => "x64"
        }}");
        if (isTiny)
        {
            _sb.Append(string.Join(" ", tiny));
        }
        foreach (var r in reference)
        {
            _sb.Append($" -r \"{r}\"");
        }
        try
        {
            var process = Process.Start(GFlat.GetProcessStartInfo(BFlat, _sb.ToString()));
            process!.WaitForExit();
            Logger.WriteLineInfo(process.StandardOutput.ReadToEnd());
        }
        catch (Exception e)
        {
            Logger.WriteLineError(e.Message);
            Logger.WriteLineError("Failed to build BFlat: " + input);
            return false;
        }


        Logger.WriteLineInfo("BFlat build complete");

        _sb.Clear();
        return true;
    }
}
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
    public static bool Build(string input, string output, TargetPlatform targetPlatform, Architecture architecture, ReadOnlySpan<string> reference, BuildType buildType, bool isTiny = false)
    {
        if (!HasBFlat())
        {
            return false;
        }
        var outputFile = $"{Path.GetFileNameWithoutExtension(input)}.{targetPlatform.GetNameLower()}.{buildType.GetNameLower()}.{architecture.GetNameLower()}.{targetPlatform switch
        {
            TargetPlatform.Windows => "dll",
            TargetPlatform.Linux   => "so",
            _                      => "so"
        }}";
        _sb.Append($"build  ./{Path.GetFileName(input)} -o:{outputFile} --target:shared");
        _sb.Append($" --os:{targetPlatform switch
        {
            TargetPlatform.Windows => "windows",
            TargetPlatform.Linux   => "linux",
            _                      => "linux"
        }}");
        _sb.Append($" --arch:{architecture switch
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
            _sb.Append($" -r:{Path.GetRelativePath(Path.GetDirectoryName(input)!, r)}");
        }
        try
        {
            var args = _sb.ToString();
            Logger.WriteLineInfo("Building BFlat: " + args);
            var info = GFlat.GetProcessStartInfo(BFlat, args);
            info.WorkingDirectory = Path.GetDirectoryName(input)!;
            var process = Process.Start(info);
            process!.WaitForExit();
            var error = process.StandardError.ReadToEnd();
            if (!string.IsNullOrWhiteSpace(error))
            {
                Logger.WriteLineError(error);
                Logger.WriteLineError("Failed to build BFlat:  bflat " + input);

                return false;
            }
            Logger.WriteLineInfo(process.StandardOutput.ReadToEnd());
            var outputFilePath = Path.Combine(output,                        outputFile);
            var buildPath      = Path.Combine(Path.GetDirectoryName(input)!, outputFile);
            File.Copy(buildPath, outputFilePath, true);
            Logger.WriteLineInfo($"bflat build output: {buildPath} built to {outputFilePath}");
        }
        catch (Exception e)
        {
            Logger.WriteLineError(e.Message);
            Logger.WriteLineError("Failed to build BFlat: bflat " + input);
            return false;
        }


        Logger.WriteLineInfo("BFlat build complete");

        _sb.Clear();
        return true;
    }
}
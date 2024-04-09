using System.Diagnostics;
using System.Text;
using CommandLine;
using GFlat.BFlat;
using GFlat.CommandLine;
using GFlat.Dotnet;
using GFlat.GDExtension;
using GFlat.Loggers;
using Tomlet;

namespace GFlat;

public enum PathType
{
    File,
    Directory
}

public class GFlat
{
    private static ILogger Logger => ConsoleLogger.Unicode;


    private static void Main(string[] args)
    {
        if (!BFlatBuilder.HasBFlat() || !DotNetBuilder.HasDotnet())
        {
            return;
        }
        Parser.Default.ParseArguments<CommandLine.Option>(args).WithParsed(option =>
            {
                var fullPath = Path.GetFullPath(option.Path);
                var directory = Path.GetDirectoryName(fullPath)!;
                Logger.WriteLineInfo("Building...");
                Logger.WriteLineInfo($"Directory: {directory}");
                Logger.WriteLineInfo($"Directory: {fullPath}");
                Logger.WriteLineInfo($"Input: {option.Path}");
                var pathType = PathType.File;
                if (File.Exists(fullPath))
                {
                    pathType = PathType.File;
                }
                else if (Directory.Exists(fullPath))
                {
                    pathType = PathType.Directory;
                }
                else
                {
                    Logger.WriteLineError("File or directory not found: " + fullPath);
                    return;
                }
                if (option.Tiny)
                {
                    Logger.WriteLineInfo("Building tiny");
                }
                else
                {

                    Logger.WriteLineInfo("Building normal");
                }
            }
        );
    }


    public static ProcessStartInfo GetProcessStartInfo(string filename, params string[] args) => new ProcessStartInfo(filename, string.Join(" ", args))
    {
        RedirectStandardOutput = true,
        RedirectStandardInput = true,
        RedirectStandardError = true,
        CreateNoWindow = true,
        StandardOutputEncoding = Encoding.UTF8,
        StandardErrorEncoding = Encoding.UTF8,
        StandardInputEncoding = Encoding.UTF8
    };
}
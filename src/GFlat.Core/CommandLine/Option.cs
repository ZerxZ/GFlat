using CommandLine;
using GFlat.GDExtension;

namespace GFlat.CommandLine;

public class Option
{
    public static Dictionary<string, (TargetPlatform, Architecture)> PlatformArchitecture = new Dictionary<string, (TargetPlatform, Architecture)>
    {
        { "win-x86", (TargetPlatform.Linux, Architecture.X86_32) },
        { "win-x64", (TargetPlatform.Windows, Architecture.X86_64) },
        { "linux-x86", (TargetPlatform.Linux, Architecture.X86_32) },
        { "linux-x64", (TargetPlatform.Linux, Architecture.X86_64) },
        { "linux-arm64", (TargetPlatform.Linux, Architecture.Arm64) },
        { "linux-rv64", (TargetPlatform.Linux, Architecture.Rv64) },
        { "macos-x64", (TargetPlatform.MacOs, Architecture.X86_64) },
        { "android-arm64", (TargetPlatform.Android, Architecture.Arm64) },
        { "android-x86", (TargetPlatform.Android, Architecture.X86_32) },
        { "android-x64", (TargetPlatform.Android, Architecture.X86_64) },
        { "ios-arm64", (TargetPlatform.Ios, Architecture.Arm64) },
        { "ios-x64", (TargetPlatform.Ios, Architecture.X86_64) },
    };
    public static bool TryGetPlatformArchitecture(string platform, out (TargetPlatform, Architecture) value)
    {
        value = default;
        return PlatformArchitecture.TryGetValue(platform.ToLower(), out value);
    }
    [Value(0, MetaName = "build-mode", Required = true, HelpText = "<build|build-il>")]
    public string BuildMode { get; set; }
    [Value(1, MetaName = "input", Required = true, HelpText = "Input project path")]
    public string InputPath { get; set; }

    [Option("tiny", Required = false, HelpText = "Tiny option")]
    public bool Tiny { get; set; }
    [Option("addon", Required = false, HelpText = "Addon option")]
    public bool Addon { get; set; }

    [Option("release", Required = false, HelpText = "<Release|Debug|All>")]
    public string Release { get; set; }
    [Option('p', "platform",
        Required = false,
        HelpText = "<win-x86|win-x64|linux-x86|linux-x64|linux-arm64|linux-rv64|macos-x64|android-arm64|android-x86|android-x64|ios-arm64|ios-x64>")]
    public IEnumerable<string> Platform { get; set; } = new List<string>();
    public string   FullPath      => Path.GetFullPath(InputPath);
    public string   DirectoryPath => Path.GetDirectoryName(FullPath)!;
    public string   FileName      => Path.GetFileNameWithoutExtension(FullPath);
    public PathType PathType      => File.Exists(FullPath) ? PathType.File : Directory.Exists(FullPath) ? PathType.Directory : PathType.None;


}
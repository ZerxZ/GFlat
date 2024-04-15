using GFlat.BFlat;
using GFlat.CommandLine;
using GFlat.Dotnet;
using GFlat.GDExtension;
using GFlat.Loggers;

namespace GFlat
{
    public class GFlatBuilder
    {
        public Option  Option { get; set; }
        public ILogger Logger => ConsoleLogger.Unicode;

        public void Execute()
        {
            if (Option.Tiny)
            {
                Logger.WriteLineInfo("Tiny option is enabled.");
            }
            if (Option.Release == "Release")
            {
                Logger.WriteLineInfo("Release option is enabled.");
            }
            Logger.WriteLineInfo($"Input path: {Option.InputPath}");
            switch (Option.PathType)
            {
                case PathType.File:
                    Logger.WriteLineInfo("Path type: File");
                    var directoryPath = Option.DirectoryPath;
                    Logger.WriteLineInfo($"Directory path: {directoryPath}");
                    var tempPath = Path.GetTempFileName();
                    File.Delete(tempPath);
                    Directory.CreateDirectory(tempPath);

                    DotNetBuilder.Build(directoryPath, tempPath, BuildType.Debug);
                    var csharpFiles = DotNetBuilder.GetCsharpFiles(directoryPath);
                    var info        = CsharpParser.ParseFiles(csharpFiles);
                    if (info == null)
                    {
                        Logger.WriteLineError("Godot initializer not found.");
                        return;
                    }
                    var generatedScript = GdExtensionBuilder.GetExtensionGeneratedScript(info);
                    var csPath          = Path.Combine(tempPath, $"{Option.FileName}.cs");
                    File.WriteAllText(csPath, generatedScript);

                    // BFlatBuilder.Build(csPath, Option.OutputPath, TargetPlatform.Windows, Architecture.X86_64, DotNetBuilder.GetDll(tempPath), BuildType.Debug, Option.Tiny);
                    // Directory.Delete(temp, true);
                    break;
                case PathType.Directory:
                    Logger.WriteLineInfo("Path type: Directory");
                    break;
                case PathType.None:
                    Logger.WriteLineError("Path type: None");
                    Logger.WriteLineError("Invalid path.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
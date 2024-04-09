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
            Logger.WriteLineInfo($"Input path: {Option.InpurPath}");
            Logger.WriteLineInfo($"Output path: {Option.OutputPath}");
            switch (Option.PathType)
            {
                case PathType.File:
                    Logger.WriteLineInfo("Path type: File");
                    var directoryPath = Option.DirectoryPath;
                    Logger.WriteLineInfo($"Directory path: {directoryPath}");
                    var temp = Path.Combine(directoryPath, "temp");
                    if (Directory.Exists(temp))
                    {
                        Directory.Delete(temp, true);
                    }

                    DotNetBuilder.Build(directoryPath, temp, BuildType.Debug);
                    var csharpFiles = DotNetBuilder.GetCsharpFiles(directoryPath);
                    var info        = csharpFiles.ToArray().Select(cs => CsharpParser.Parse(File.ReadAllText(cs))).FirstOrDefault(x => x != null);
                    if (info == null)
                    {
                        Logger.WriteLineError("Godot initializer not found.");
                        return;
                    }
                    var generatedScript = GdExtensionBuilder.GetExtensionGeneratedScript(info);
                    var csPath          = Path.Combine(temp, $"{Option.FileName}.cs");
                    File.WriteAllText(csPath, generatedScript);
                
                    BFlatBuilder.Build(csPath, Option.OutputPath, TargetPlatform.Windows, Architecture.X86_64, DotNetBuilder.GetDll(temp), BuildType.Debug, Option.Tiny);
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
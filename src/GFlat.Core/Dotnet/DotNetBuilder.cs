using System.Diagnostics;
using GFlat.GDExtension;
using GFlat.Loggers;

namespace GFlat.Dotnet;

public static class DotNetBuilder
{
    public const  string  Dotnet     = "dotnet";
    public static bool    InitDotnet = false;
    public static ILogger Logger => ConsoleLogger.Unicode;
    public static bool HasDotnet()
    {
        if (InitDotnet)
        {
            return true;
        }
        try
        {

            var process = Process.Start(GFlat.GetProcessStartInfo(Dotnet, "--version"));
            process!.WaitForExit();

        }
        catch (Exception e)
        {
            Logger.WriteLineError("Dotnet not found: " + e.Message);
            Logger.WriteLineError("Please install dotnet sdk from https://dotnet.microsoft.com/download");
            return false;
        }
        InitDotnet = true;
        return true;
    }
    public static bool Build(string projectPath, string outputPath, BuildType buildType,Action<string,string,BuildType> onBuild)
    {
        if (!HasDotnet())
        {
            return false;
        }
        Logger.WriteLineInfo("Building project: " + projectPath);
        Logger.WriteLineInfo("Output path: " + outputPath);
        Logger.WriteLineInfo("Build type: " + buildType.GetName());
        Logger.WriteLineInfo($"Directory: {Path.GetDirectoryName(projectPath)}");
        Logger.WriteLineInfo("Building project...");
        try
        {
            var process = Process.Start(GFlat.GetProcessStartInfo(Dotnet, $"publish \"{projectPath}\" -c {buildType.GetName()} -o \"{outputPath}\""));
            process!.WaitForExit();
            Logger.WriteLineInfo("Project built successfully");
            onBuild(projectPath, outputPath, buildType);
        }
        catch (Exception e)
        {
            Logger.WriteLineError(e.Message);
            Logger.WriteLineError("Failed to build project: " + projectPath);
            return false;
        }
        
        return true;
    }
    public static ReadOnlySpan<string> SearchProjects(string path, string searchPattern = "*.csproj")
    {
        return Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories).ToArray();
    }
    public static bool BuildAll(string path, string outputPath, BuildType buildType, Action<string, string, BuildType> onBuild)
    {
        Logger.WriteLineInfo("Building all projects in: " + path);
        var projects = GetProjects(path);
        foreach (var project in projects)
        {
            Logger.WriteLineInfo("Building project: " + project);
            if (!HasPackage(project, "GFlat.Godot", "Godot.Bindings"))
            {
                Logger.WriteLineInfo($"Dotnet project {project} does not have Godot.Bindings or GFlat.Godot package installed. Skipping...");
                continue;
            }

            if (Build(project, outputPath, buildType, onBuild))
            {
                continue;
            }
            Logger.WriteLineError("Failed to build project: " + project);
            return false;
        }
        Logger.WriteLineInfo("All projects built successfully");
        return true;
    }
    public static ReadOnlySpan<string> GetProjects(string path, string searchPattern = "*.csproj")
    {

        return SearchProjects(path, searchPattern).ToArray().Select(Path.GetDirectoryName).ToArray()!;
    }
    public static ReadOnlySpan<string> GetDll(string path)
    {
        return Directory.EnumerateFiles(path, "*.dll", SearchOption.AllDirectories).ToArray();
    }
    public static bool HasPackage(string path, params string[] packages)
    {
        var info = GFlat.GetProcessStartInfo(Dotnet, $"list  package -- format json");
        info.WorkingDirectory = path;
        var process = Process.Start(info);
        process!.WaitForExit();
        var output = process.StandardOutput.ReadToEnd();
        return packages.All(package => output.Contains(package));
    }
}
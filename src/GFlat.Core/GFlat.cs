using System.Diagnostics;
using System.Text;
using CommandLine;
using GFlat.BFlat;
using GFlat.CommandLine;
using GFlat.Dotnet;
using GFlat.GDExtension;
using GFlat.Loggers;
using Godot;
using Newtonsoft.Json;
using Tomlet;

namespace GFlat;

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
                Logger.WriteLineInfo($"JSON: {JsonConvert.SerializeObject(option)}");
                var builder = new GFlatBuilder
                {
                    Option = option
                };

                builder.Execute();

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
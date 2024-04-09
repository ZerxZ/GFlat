using CommandLine;
using GFlat.GDExtension;

namespace GFlat.CommandLine;

public class Option
{
    [Option("tiny", Required = false, HelpText = "Tiny option")]
    public bool Tiny { get; set; }
    [Value(0, MetaName = "path", Required = true, HelpText = "Input file or directory path.")]
    public string Path { get; set; }
    [Value(1, MetaName = "output", Required = false, HelpText = "Output path.")]
    public string OutputPath { get; set; }

    [Option("release", Required = false, HelpText = "Release option")]
    public bool Release { get; set; }
}
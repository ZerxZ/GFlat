using System.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Text.RegularExpressions;
using GFlat.Loggers;

namespace GFlat.Dotnet
{
    public class GFlatGodotInfo
    {
        public string Namespace;
        public string ClassName;
        public string Initializer;
        public string Terminator;
        public string EntrySymbol;
        public string InitializationLevel;
        public string CompatibilityMinimum;
        public string CompatibilityMaximum;
        public string Reloadable;
    }

    public partial class CsharpParser
    {
        [GeneratedRegex("(GodotInitializer)")]
        public static partial Regex GodotInitializerRegex();
        public static ILogger Logger => ConsoleLogger.Unicode;
        public static GFlatGodotInfo? ParseFiles(ReadOnlySpan<string> files)
        {

            var regex = GodotInitializerRegex();
            foreach (var file in files)
            {
                var code = File.ReadAllText(file);
                if (!regex.Match(code).Success)
                {
                    continue;
                }
                var info = Parse(code);
                if (info != null)
                {
                    return info;
                }
            }

            return null;
        }
        public static GFlatGodotInfo? ParseFiles(IEnumerable<string> files)
        {

            var regex = GodotInitializerRegex();
            foreach (var file in files)
            {
                var code = File.ReadAllText(file);
                if (!regex.Match(code).Success)
                {
                    continue;
                }
                var info = Parse(code);
                if (info != null)
                {
                    return info;
                }
            }

            return null;
        }
        public static GFlatGodotInfo? Parse(string code)
        {
            Logger.WriteLineInfo("Parsing C# code...");
            Logger.WriteLineInfo(code[..22]);

            var tree      = CSharpSyntaxTree.ParseText(code, new CSharpParseOptions(LanguageVersion.Latest));
            var root      = tree.GetCompilationUnitRoot();
            var rootNodes = root.DescendantNodes();

            var @namespace = rootNodes.OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault()?.Name ?? rootNodes.OfType<NamespaceDeclarationSyntax>().FirstOrDefault()?.Name;

            var classDeclarationSyntaxes = rootNodes.OfType<ClassDeclarationSyntax>();

            foreach (var node in classDeclarationSyntaxes)
            {

                var attributes       = node.AttributeLists.SelectMany(x => x.Attributes);
                var godotInitializer = attributes.FirstOrDefault(a => a.Name.ToString().EndsWith("GodotInitializer"));

                if (godotInitializer == null)
                    continue;

                var info = new GFlatGodotInfo
                {
                    ClassName = node.Identifier.ToString().Trim(),
                    EntrySymbol = godotInitializer.ArgumentList!.Arguments[0].ToString()[1..^1],
                    InitializationLevel = godotInitializer.ArgumentList!.Arguments[1].ToString()
                };
                if (@namespace is not null)
                {
                    info.Namespace = @namespace.ToString().Trim();
                }
                var arguments = godotInitializer.ArgumentList!.Arguments;
                if (arguments.Count > 2)
                {
                    var args = arguments.ToArray()[2..];
                    foreach (var argument in args)
                    {
                        var split = argument.ToString().Split('=');
                        var name  = split[0].Trim();
                        var value = split[1].Trim();
                        switch (name)
                        {
                            case "CompatibilityMinimum":
                                info.CompatibilityMinimum = value[1..^1];
                                break;
                            case "CompatibilityMaximum":
                                info.CompatibilityMaximum = value[1..^1];
                                break;
                            case "Reloadable":
                                info.Reloadable = value;
                                break;
                        }
                    }
                }

                var methodDeclarationSyntaxes = node.Members.OfType<MethodDeclarationSyntax>();
                foreach (var method in methodDeclarationSyntaxes)
                {
                    var methodAttributes = method.AttributeLists.SelectMany(x => x.Attributes);


                    var bindInitializer = methodAttributes.FirstOrDefault(a => a.Name.ToString().EndsWith("BindInitializer"));
                    var bindTerminator  = methodAttributes.FirstOrDefault(a => a.Name.ToString().EndsWith("BindTerminator"));

                    if (bindInitializer != null)
                        info.Initializer = method.Identifier.ToString().Trim();

                    if (bindTerminator != null)
                        info.Terminator = method.Identifier.ToString().Trim();

                }

                return info;
            }

            return null;
        }
    }
}
using System.Text;
using GFlat.Dotnet;

namespace GFlat.GDExtension;

public static class GdExtensionBuilder
{
    public const   string        ExtensionExt = ".gdextension";
    private static StringBuilder _sb          = new StringBuilder();
    public static string GetExtensionGeneratedScript(GFlatGodotInfo info)
    {

        return GetExtensionGeneratedScript(info.Namespace, info.ClassName, info.EntrySymbol, info.InitializationLevel, info.Initializer, info.Terminator);
    }
    public static string GetExtensionGeneratedScript(string @namespace, string @classname, string @entrySymbol, string @level, string @initiallizer, string @terminator)
    {
        _sb.Clear();
        _sb.AppendLine($@"
using Godot.Bridge;
using System.Runtime.InteropServices;
namespace {@namespace}.GFlat;
public class {@classname}GFlat{{
        // Initialization
        [UnmanagedCallersOnly(EntryPoint = ""{@entrySymbol}"")]
        public static bool Initialize(nint getProcAddress, nint library, nint initialization)
        {{
            GodotBridge.Initialize(getProcAddress, library, initialization, config =>
            {{
            config.SetMinimumLibraryInitializationLevel(global::Godot.Bridge.{@level});
");
        if (!string.IsNullOrEmpty(@initiallizer))
        {
            _sb.Append($"config.RegisterInitializer(global::");
            if (!string.IsNullOrEmpty(@namespace))
            {
                _sb.Append($"{@namespace}.");
            }
            _sb.AppendLine($"{@classname}.{@initiallizer});");
        }
        if (!string.IsNullOrEmpty(@initiallizer))
        {
            _sb.Append($"config.RegisterTerminator(global::");
            if (!string.IsNullOrEmpty(@namespace))
            {
                _sb.Append($"{@namespace}.");
            }
            _sb.AppendLine($"{@classname}.{@terminator});");
        }
        _sb.AppendLine($$$"""
                          
                                      });
                                      return true;
                                  }}
                          """);
        var result = _sb.ToString();
        _sb.Clear();
        return result;
    }

}
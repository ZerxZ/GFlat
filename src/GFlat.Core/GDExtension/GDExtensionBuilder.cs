namespace GFlat.GDExtension;

public static class GdExtensionBuilder
{
    public const string ExtensionExt = ".gdextension";
    public static string GetExtensionGeneratedScript(string @namespace, string @classname, string @entrySymbol, string @level, string @initiallizer, string @terminator)
    {
        return $$"""
                 using Godot.Bridge;
                 using System.Runtime.InteropServices;
                 namespace {{@namespace}}.GFlat {
                     public class {{@classname}}{
                         // Initialization
                         [UnmanagedCallersOnly(EntryPoint = "{{@entrySymbol}}")]
                         public static bool Initialize(nint getProcAddress, nint library, nint initialization)
                         {
                             GodotBridge.Initialize(getProcAddress, library, initialization, config =>
                             {
                                 config.SetMinimumLibraryInitializationLevel(global::{{@level}});
                                 config.RegisterInitializer(global::{{@initiallizer}});
                                 config.RegisterTerminator(global::{{@terminator}});
                             });
                             return true;
                         }
                     }
                 }
                 """;
    }

}
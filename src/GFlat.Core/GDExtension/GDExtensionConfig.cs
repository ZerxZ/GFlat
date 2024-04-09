using Salaros.Configuration;

namespace GFlat.GDExtension;

public class GdExtensionConfig
{


    public GdExtensionConfiguration Configuration { get; set; } = new GdExtensionConfiguration("godot_gdextension_initialize");

    public          Dictionary<string, string> Libraries  { get; set; } = new Dictionary<string, string>();
    public override string                     ToString() => ToCfg(this);
    public void RegisterLibrary(string platform, string path)
    {
        Libraries[platform] = path;
    }
    public void RegisterLibrary(TargetPlatform targetPlatform, Architecture architecture, BuildType buildType, string path)
    {
        Libraries[$"{targetPlatform.GetNameLower()}.{buildType.GetNameLower()}.{architecture.GetNameLower()}"] = path;
    }
    public static GdExtensionConfig Parse(string iniConfig)
    {
        var gdExtensionConfig = new GdExtensionConfig();
        var parser            = new ConfigParser(iniConfig);
        gdExtensionConfig.Configuration.EntrySymbol = parser.GetValue("configuration",          "entry_symbol");
        gdExtensionConfig.Configuration.CompatibilityMinimum = parser.GetValue("configuration", "compatibility_minimum");
        var compatibilityMaximum = parser.GetValue("configuration", "compatibility_maximum");
        if (compatibilityMaximum != null)
        {
            gdExtensionConfig.Configuration.CompatibilityMaximum = compatibilityMaximum;
        }
        var reloadable = parser.GetValue("configuration", "reloadable");
        if (reloadable != null)
        {
            gdExtensionConfig.Configuration.Reloadable = bool.Parse(reloadable);
        }

        foreach (var key in parser["libraries"].Keys)
        {
            var value = parser.GetValue("libraries", key.Name);
            gdExtensionConfig.Libraries.TryAdd(key.Name, value);
        }
        return gdExtensionConfig;
    }
    public static void ToCfg(GdExtensionConfig config, string path)
    {
        var parser = new ConfigParser(path);
        parser.SetValue("configuration", "entry_symbol",          config.Configuration.EntrySymbol);
        parser.SetValue("configuration", "compatibility_minimum", config.Configuration.CompatibilityMinimum);

        if (config.Configuration.CompatibilityMaximum != null)
        {
            parser.SetValue("configuration", "compatibility_maximum", config.Configuration.CompatibilityMaximum);
        }
        if (config.Configuration.Reloadable.HasValue)
        {
            parser.SetValue("configuration", "reloadable", config.Configuration.Reloadable.Value);

        }
        foreach (var library in config.Libraries)
        {
            parser.SetValue("libraries", library.Key, library.Value);
        }
        parser.Save();
    }
    public static string ToCfg(GdExtensionConfig config)
    {
        var parser = new ConfigParser();
        parser.SetValue("configuration", "entry_symbol",          config.Configuration.EntrySymbol);
        parser.SetValue("configuration", "compatibility_minimum", config.Configuration.CompatibilityMinimum);
        if (config.Configuration.CompatibilityMaximum != null)
        {
            parser.SetValue("configuration", "compatibility_maximum", config.Configuration.CompatibilityMaximum);
        }
        if (config.Configuration.Reloadable.HasValue)
        {
            parser.SetValue("configuration", "reloadable", config.Configuration.Reloadable.Value);

        }
        foreach (var library in config.Libraries)
        {
            parser.SetValue("libraries", library.Key, library.Value);
        }
        return parser.ToString();
    }
}

public class GdExtensionConfiguration
{

    public GdExtensionConfiguration(string entrySymbol, string compatibilityMinimum = "4.1", bool? reloadable = null)
    {
        EntrySymbol = entrySymbol;
        CompatibilityMinimum = compatibilityMinimum;
        Reloadable = reloadable;
    }
    public string EntrySymbol          { get; set; }
    public string CompatibilityMinimum { get; set; }

    public string? CompatibilityMaximum { get; set; } = null;
    public bool?   Reloadable           { get; set; } = null;
}
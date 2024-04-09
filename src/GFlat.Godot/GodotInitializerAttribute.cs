using Godot.Bridge;

namespace GFlat.Godot;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class GodotInitializerAttribute : Attribute
{
    public GodotInitializerAttribute(string entrySymbol) => EntrySymbol = entrySymbol;
    public GodotInitializerAttribute(string entrySymbol, InitializationLevel initializationLevel) : this(entrySymbol) => InitializationLevel = initializationLevel;

    public InitializationLevel InitializationLevel  { get; } = InitializationLevel.Core;
    public string              EntrySymbol          { get; }
    public string?             CompatibilityMinimum { get; set; } = "4.1";
    public string?             CompatibilityMaximum { get; set; } = null;
    public bool?               Reloadable           { get; set; } = false;

}
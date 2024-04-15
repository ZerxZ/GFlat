using GFlat.Godot;
using Godot.Bridge;

namespace GFlat
{

    [GodotInitializer("GFlat", InitializationLevel.Scene, CompatibilityMinimum = "4.1", CompatibilityMaximum = "123", Reloadable = false)]
    public partial class Test
    {
        [BindInitializer]
        public static void Init(InitializationLevel level)
        {
            Console.WriteLine("Hello from GFlat");
        }
        [BindTerminator]
        public static void Term(InitializationLevel level)
        {
            Console.WriteLine("Goodbye from GFlat");

        }
    }
}
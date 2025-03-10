﻿namespace GFlat.GDExtension;



public enum PathType
{
    File,
    Directory,
    None
}

public enum TargetPlatform
{
    Windows,
    Linux,
    MacOs,
    Android,
    Ios,
}

public enum Architecture
{
    X86_32,
    X86_64,
    Arm64,
    Rv64
}

public enum BuildType
{
    Debug,
    Release,
    All
}

public static class GdExtensionHelper
{


    public static string GetName<T>(this T value) where T : Enum
    {
        return Enum.GetName(typeof(T), value)!;
    }
    public static string GetNameLower<T>(this T value) where T : Enum
    {
        return value.GetName()!.ToLower();
    }
}
using System.IO;

namespace Snoopy.Windows.Services;

public static class AssetLocator
{
    private static string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;
    private static string AssetsDirectory => Path.Combine(BaseDirectory, "Assets");

    public static string VideosDirectory => Path.Combine(AssetsDirectory, "Videos");
    public static string BackgroundsDirectory => Path.Combine(AssetsDirectory, "Backgrounds");
    public static string HalftonePatternPath => Path.Combine(AssetsDirectory, "Patterns", "halftone_pattern.png");
}

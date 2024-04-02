namespace JetBrainsVersionUp.Models;

public class JetBrainsProductInfo
{
    public string? name { get; set; }
    public string? version { get; set; }
    public string? buildNumber { get; set; }
    public string? productCode { get; set; }
    public string? dataDirectoryName { get; set; }
    public string? svgIconPath { get; set; }
    public string? productVendor { get; set; }
    public Launch[]? launch { get; set; }
    public string[]? bundledPlugins { get; set; }
    public string[]? modules { get; set; }
    public string[]? fileExtensions { get; set; }
}

public class Launch
{
    public string? os { get; set; }
    public string? arch { get; set; }
    public string? launcherPath { get; set; }
    public string? javaExecutablePath { get; set; }
    public string? vmOptionsFilePath { get; set; }
    public string[]? bootClassPathJarNames { get; set; }
    public string[]? additionalJvmArguments { get; set; }
}

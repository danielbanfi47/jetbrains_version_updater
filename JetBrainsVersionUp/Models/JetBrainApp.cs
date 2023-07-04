namespace JetBrainsVersionUp.Models;

internal class JetBrainApp
{
    public string DisplayName { get; set; }
    public string DisplayVersion { get; set; }
    public string InstallLocation { get; set; }
    public string RegistryLocation { get; set; }

    public JetBrainsProducts JetBrainsProducts { get; set; }

    public JetBrainApp(
        string? displayName,
        string? displayVersion,
        string? installLocation,
        string? registryLocation,
        JetBrainsProducts jetBrainsProducts)
    {
        DisplayName = displayName ?? string.Empty;
        DisplayVersion = displayVersion ?? string.Empty;
        InstallLocation = installLocation ?? string.Empty;
        RegistryLocation = registryLocation ?? string.Empty;
        JetBrainsProducts = jetBrainsProducts;
    }

    public override string ToString()
    {
        return $"DisplayName: {DisplayName}\nDisplayVersion: {DisplayVersion}\nInstallLocation: {InstallLocation}";
    }
}

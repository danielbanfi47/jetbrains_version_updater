using JetBrainsVersionUp.Models;
using System.Text.Json;

namespace JetBrainsVersionUp.Services;

internal class ActualVersionReader
{
    private readonly string _json_name = "product-info.json";
    
    public async Task<Result> GetActualVersion(JetBrainApp? jetBrainApp)
    {
        if (jetBrainApp == null)
        {
            return new Result("No install information");
        }
        
        var json_full_path = Path.Combine(jetBrainApp.InstallLocation, _json_name);

        if (!File.Exists(json_full_path))
        {
            return new Result("No info file in the installation folder");
        }
 
        try
        {
            using FileStream openStream = File.OpenRead(json_full_path);
            var app_info = await JsonSerializer.DeserializeAsync<JetBrainsProductInfo>(openStream);
            if (app_info == null)
            {
                return new Result("Invalid installation");
            }

            if (IsActualDifferent(
                jetBrainApp.DisplayName,
                jetBrainApp.DisplayVersion,
                jetBrainApp.JetBrainsProducts,
                app_info.version ?? string.Empty,
                app_info.buildNumber ?? string.Empty))
            {
                return new Result(new List<JetBrainApp>{new JetBrainApp(
                    jetBrainApp.DisplayName.Replace(GetVersionFromName(jetBrainApp.DisplayName, jetBrainApp.JetBrainsProducts), app_info.version),
                    app_info.buildNumber,
                    jetBrainApp.InstallLocation,
                    jetBrainApp.RegistryLocation,
                    jetBrainApp.JetBrainsProducts)});
            }

            return new Result("Nothing changed", true);
        }
        catch (Exception ex) 
        { 
            return new Result(ex.Message); 
        }
    }

    private bool IsActualDifferent(
        string jetBrainNameFromRegistry,
        string jetBrainVersionFromRegistry,
        JetBrainsProducts jetBrainsProductFromRegistry,
        string jetBrainVersionFromJson,
        string jetBrainBuildVersionFromJson)
    {

        var versionFromName = GetVersionFromName(jetBrainNameFromRegistry, jetBrainsProductFromRegistry);
        return
            versionFromName != jetBrainVersionFromJson ||
            jetBrainVersionFromRegistry != jetBrainBuildVersionFromJson;
    }

    private string GetVersionFromName(
        string jetBrainNameFromRegistry, 
        JetBrainsProducts jetBrainsProductFromRegistry)
    {
        return 
            jetBrainNameFromRegistry.Substring(
            jetBrainNameFromRegistry.IndexOf(jetBrainsProductFromRegistry.ToString()) +
            jetBrainsProductFromRegistry.ToString().Length + 1).Trim();
    }
}

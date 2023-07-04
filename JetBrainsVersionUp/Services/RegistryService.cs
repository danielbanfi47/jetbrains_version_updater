using JetBrainsVersionUp.Models;
using Microsoft.Win32;
using System.Security;
using System.Runtime.Versioning;

namespace JetBrainsVersionUp.Services;

internal class RegistryService
{
    private readonly string _uninstall_path = "SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\";
    private readonly string _display_name = "DisplayName";
    private readonly string _display_version = "DisplayVersion";
    private readonly string _install_location = "InstallLocation";


    [SupportedOSPlatform("windows")]
    public Result FindIntellyJInRegistry(JetBrainsProducts jetBrainsJProducts)
    {
        try
        {
            var key = Registry.LocalMachine.OpenSubKey(_uninstall_path, false);
            if (key == null)
            {
                return new Result("Registry in invalid");
            }

            var uninstall_apps = key.GetSubKeyNames().ToList();
            var founded_apps = uninstall_apps.Where(ua => ua.Contains(jetBrainsJProducts.ToString())).ToList();
            if (founded_apps.Count == 0)
            {
                return new Result("Nothing uninstall information in the registry");
            }

            var resultList = new List<JetBrainApp>();

            foreach (var f_app in founded_apps)
            {
                var full_registry_path = $"{_uninstall_path}{f_app}";
                var intelly_app = Registry.LocalMachine.OpenSubKey(full_registry_path, false);
                var intelly_app_values = intelly_app?.GetValueNames().ToList();
                if (intelly_app == null ||
                    intelly_app_values == null ||
                    !intelly_app_values.Contains(_display_name) ||
                    !intelly_app_values.Contains(_display_version) ||
                    !intelly_app_values.Contains(_install_location))
                {
                    continue;
                }

                var display_name_key = intelly_app.GetValue(_display_name)?.ToString();
                var display_version_key = intelly_app.GetValue(_display_version)?.ToString();
                var install_location_key = intelly_app.GetValue(_install_location)?.ToString();

                intelly_app.Close();

                resultList.Add(new JetBrainApp(
                    display_name_key,
                    display_version_key,
                    install_location_key,
                    full_registry_path,
                    jetBrainsJProducts));
            }

            key.Close();

            return new Result(resultList);
        }
        catch (SecurityException ex)
        {
            return new Result(ex.Message);
        }
    }

    [SupportedOSPlatform("windows")]
    public Result UpdateIntellyJInRegistry(JetBrainApp? jetBrainApp)
    {
        if (jetBrainApp == null)
        {
            return new Result();
        }
        
        try
        {
            var key = Registry.LocalMachine.OpenSubKey(jetBrainApp.RegistryLocation, true);
            if (key == null)
            {
                return new Result();
            }

            key.SetValue(_display_name, jetBrainApp.DisplayName);
            key.SetValue(_display_version, jetBrainApp.DisplayVersion);

            key.Close();
        }
        catch (SecurityException ex)
        {
            Console.WriteLine(ex.Message);
            return new Result();
        }

        return new Result(new List<JetBrainApp> { jetBrainApp });
    }
}

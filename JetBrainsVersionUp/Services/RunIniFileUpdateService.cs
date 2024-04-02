using JetBrainsVersionUp.Models;
using JetBrainsVersionUp.Services.Abstraction;
using System.IO.Abstractions;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace JetBrainsVersionUp.Services;

public class RunIniFileUpdateService
{
    private readonly Serilog.Core.Logger _logger;
    private readonly IFileSystem _fileSystem;
    private readonly IRegistryService _registryService;
    private readonly string _additionalIniPath = "bin\\idea.properties";
    private readonly string _systemPathInIni = "idea.system.path=";
    private readonly string _systemPathNewBaseFolder = "c:/ws/.JetBrains";
    private readonly string _commentRegex = "^#\\s+";
    private readonly string _userHomeRegex = "\\$\\{user.home\\}";


    public RunIniFileUpdateService(IRegistryService registryService, IFileSystem fileSystem, Serilog.Core.Logger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Must be set");
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem), "Must be set");
        _registryService = registryService ?? throw new ArgumentNullException(nameof(registryService), "Must be set");
    }

    [SupportedOSPlatform("windows")]
    public async Task Run(JetBrainsProducts jetBrainsProducts)
    {
        var res = _registryService.FindIntelliJInRegistry(jetBrainsProducts);

        if (!res.Success)
        {
            _logger.Warning(res.Message);
            return;
        }

        foreach (var resApp in res.ResultApp)
        {
            var fullPath = _fileSystem.Path.Combine(resApp.InstallLocation, _additionalIniPath);
            if (!_fileSystem.File.Exists(fullPath))
            {
                _logger.Error($"Ini file not exists ({fullPath})");
                continue;
            }

            var full_file_content = new List<string>();
            try
            {
                full_file_content = _fileSystem.File.ReadAllLines(fullPath).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error($"Can not open the '{fullPath}' file ({ex.Message})");
                continue;
            }

            var systemPathLine = full_file_content.FirstOrDefault(f => f.Contains(_systemPathInIni));

            if (string.IsNullOrEmpty(systemPathLine))
            {
                _logger.Error("The ini file does not contain any system path information");
                continue;
            }

            var index_of_system_path = full_file_content.IndexOf(systemPathLine);

            systemPathLine = Regex.Replace(systemPathLine, _commentRegex, "");
            systemPathLine = Regex.Replace(systemPathLine, _userHomeRegex, _systemPathNewBaseFolder);

            if (full_file_content.Contains(systemPathLine))
            {
                _logger.Information("The final system path is already in the ini file");
                continue;
            }

            full_file_content.Insert(
                full_file_content.Count() > index_of_system_path + 1 ?
                    index_of_system_path + 1 :
                    index_of_system_path,
                systemPathLine);

            try
            {
                _fileSystem.File.Copy(fullPath, $"{fullPath}_bck", true);
            }
            catch
            {
                _logger.Warning("Can not create backup from the ini file");
            }

            try
            {
                _fileSystem.File.WriteAllLines(fullPath, full_file_content);
            }
            catch
            {
                _logger.Error("Unable save the new ini file");
                continue;
            }
            _logger.Information("Ini updated");
        }

        _logger.Information("-------------------");
    }
}

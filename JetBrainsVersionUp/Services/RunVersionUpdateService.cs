using JetBrainsVersionUp.Models;
using JetBrainsVersionUp.Services.Abstraction;
using System.Runtime.Versioning;

namespace JetBrainsVersionUp.Services;

public class RunVersionUpdateService
{
    private readonly IRegistryService _registryService;
    private readonly IActualVersionReader _actualVersionReader;
    private readonly Serilog.Core.Logger _logger;

    public RunVersionUpdateService(IRegistryService registryService, IActualVersionReader actualVersionReader, Serilog.Core.Logger logger)
    {
        _registryService = registryService ?? throw new ArgumentNullException(nameof(registryService), "Must be set");
        _actualVersionReader = actualVersionReader ?? throw new ArgumentNullException(nameof(actualVersionReader), "Must be set");
        _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Must be set");
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
            foreach (var niceMessage in resApp.GetNiceFormat())
            {
                _logger.Information(niceMessage);
            }

            var vinfo = await _actualVersionReader.GetActualVersion(resApp);

            if (!vinfo.Success)
            {
                _logger.Warning(vinfo.Message);
                continue;
            }

            if (vinfo.ResultApp.Count != 1)
            {
                if (vinfo.Success)
                {
                    _logger.Information(vinfo.Message);
                }
                else
                {
                    _logger.Warning("Invalid actual version");
                }
                continue;
            }

            foreach (var niceMessage in vinfo.ResultApp[0].GetNiceFormat())
            {
                _logger.Information(niceMessage);
            }

            _registryService.UpdateIntelliJInRegistry(vinfo.ResultApp[0]);
        }

        _logger.Information("-------------------");
    }
}

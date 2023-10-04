using JetBrainsVersionUp.Models;
using System.Runtime.Versioning;

namespace JetBrainsVersionUp.Services
{
    internal class RunService
    {
        private readonly RegistryService _registryService;
        private readonly ActualVersionReader _actualVersionReader;
        private readonly Serilog.Core.Logger _logger;

        public RunService(RegistryService registryService, ActualVersionReader actualVersionReader, Serilog.Core.Logger logger)
        {
            _registryService = registryService ?? throw new ArgumentNullException(nameof(_registryService), "Must be set");
            _actualVersionReader = actualVersionReader ?? throw new ArgumentNullException(nameof(_actualVersionReader), "Must be set");
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger), "Must be set");
        }

        [SupportedOSPlatform("windows")]
        public async Task Run(JetBrainsProducts jetBrainsProducts)
        {
            var res = _registryService.FindIntellyJInRegistry(jetBrainsProducts);

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

                foreach(var niceMessage in vinfo.ResultApp[0].GetNiceFormat())
                {
                    _logger.Information(niceMessage);
                }

                _registryService.UpdateIntellyJInRegistry(vinfo.ResultApp[0]);
            }

            _logger.Information("-------------------");
        }
    }
}

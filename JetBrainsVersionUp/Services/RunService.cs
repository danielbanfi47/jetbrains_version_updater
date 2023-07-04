using JetBrainsVersionUp.Models;
using System.Runtime.Versioning;

namespace JetBrainsVersionUp.Services
{
    internal class RunService
    {
        private readonly RegistryService _registryService;
        private readonly ActualVersionReader _actualVersionReader;

        public RunService(RegistryService registryService, ActualVersionReader actualVersionReader)
        {
            _registryService = registryService;
            _actualVersionReader = actualVersionReader;
        }

        [SupportedOSPlatform("windows")]
        public async Task Run(JetBrainsProducts jetBrainsProducts)
        {
            var res = _registryService.FindIntellyJInRegistry(jetBrainsProducts);

            if (!res.Success)
            {
                Console.WriteLine(res.Message);
                return;
            }

            foreach (var resApp in res.ResultApp)
            {
                Console.WriteLine(resApp.ToString());

                var vinfo = await _actualVersionReader.GetActualVersion(resApp);

                if (!vinfo.Success)
                {
                    Console.WriteLine(vinfo.Message);
                    continue;
                }

                if (vinfo.ResultApp.Count != 1)
                {
                    Console.WriteLine("Invalid actual version");
                    continue;
                }

                Console.WriteLine(vinfo.ResultApp[0].ToString());

                _registryService.UpdateIntellyJInRegistry(vinfo.ResultApp[0]);
            }

            Console.WriteLine("-------------------");
        }
    }
}

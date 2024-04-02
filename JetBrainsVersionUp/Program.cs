using JetBrainsVersionUp.Models;
using JetBrainsVersionUp.Services;
using Serilog;
using System.IO.Abstractions;

var registryService = new RegistryService();

var runVersionUpdateService = new RunVersionUpdateService(
    registryService,
    new ActualVersionReader(),
    new LoggerConfiguration()
                           .WriteTo.Console()
                           .MinimumLevel.Debug()
                           .WriteTo.File("JetBrainVersiopnUpdater.log")
                           .CreateLogger());

var runIniFileUpdateService = new RunIniFileUpdateService(
    registryService,
    new FileSystem(),
    new LoggerConfiguration()
                           .WriteTo.Console()
                           .MinimumLevel.Debug()
                           .WriteTo.File("JetBrainVersiopnUpdater.log")
                           .CreateLogger());

await runVersionUpdateService.Run(JetBrainsProducts.PyCharm);
await runIniFileUpdateService.Run(JetBrainsProducts.PyCharm);
await runVersionUpdateService.Run(JetBrainsProducts.IDEA);
await runIniFileUpdateService.Run(JetBrainsProducts.IDEA);
Thread.Sleep(2500);

using JetBrainsVersionUp.Models;
using JetBrainsVersionUp.Services;
using Serilog;

var runService = new RunService(
    new RegistryService(), 
    new ActualVersionReader(),
    new LoggerConfiguration()
                           .WriteTo.Console()
                           .MinimumLevel.Debug()
                           .WriteTo.File("JetBrainVersiopnUpdater.log")
                           .CreateLogger()); 

await runService.Run(JetBrainsProducts.PyCharm);
await runService.Run(JetBrainsProducts.IDEA);
Thread.Sleep(2500);

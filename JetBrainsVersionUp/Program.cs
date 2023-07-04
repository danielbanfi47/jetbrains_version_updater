using JetBrainsVersionUp.Models;
using JetBrainsVersionUp.Services;

var runService = new RunService(new RegistryService(), new ActualVersionReader()); 

await runService.Run(JetBrainsProducts.PyCharm);
await runService.Run(JetBrainsProducts.IDEA);
Thread.Sleep(2500);

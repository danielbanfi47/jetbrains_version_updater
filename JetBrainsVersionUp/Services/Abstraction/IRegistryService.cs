using JetBrainsVersionUp.Models;

namespace JetBrainsVersionUp.Services.Abstraction
{
    public interface IRegistryService
    {
        Result FindIntelliJInRegistry(JetBrainsProducts jetBrainsJProducts);
        Result UpdateIntelliJInRegistry(JetBrainApp? jetBrainApp);
    }
}
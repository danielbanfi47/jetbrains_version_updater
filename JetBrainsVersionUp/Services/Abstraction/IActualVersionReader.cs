using JetBrainsVersionUp.Models;

namespace JetBrainsVersionUp.Services.Abstraction
{
    public interface IActualVersionReader
    {
        Task<Result> GetActualVersion(JetBrainApp? jetBrainApp);
    }
}
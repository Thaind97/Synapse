using System.Collections.Generic;
using System.Threading.Tasks;

namespace Synapse.Services.Services.Abstraction
{
    public interface IRunManager
    {
        Task EnqueueRunAsync(long sequenceId, string batteryId);
        Task StopRunAsync(string batteryId);
        IReadOnlyDictionary<string, string> GetStatuses();
    }
}

using System.Threading;
using System.Threading.Tasks;
using Synapse.Infrastructure.Entities;

namespace Synapse.Services.Services.Abstraction
{
    public interface ICommandExecutor
    {
        Task ExecuteAsync(DeviceCommand command, string batteryId, CancellationToken cancellationToken = default);
    }
}

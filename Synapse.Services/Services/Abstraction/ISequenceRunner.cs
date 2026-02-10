using System.Threading;
using System.Threading.Tasks;

namespace Synapse.Services.Services.Abstraction
{
    public interface ISequenceRunner
    {
        /// <summary>
        /// Run sequence for given sequenceId and batteryId. Method returns when run finishes or cancellation requested.
        /// </summary>
        Task RunAsync(long sequenceId, string batteryId, CancellationToken cancellationToken = default);
    }
}

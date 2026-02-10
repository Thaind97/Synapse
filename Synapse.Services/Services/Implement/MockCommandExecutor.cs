using System.Threading;
using System.Threading.Tasks;
using Synapse.Services.Services.Abstraction;
using Synapse.Infrastructure.Entities;
using Microsoft.Extensions.Logging;

namespace Synapse.Services.Services.Implement
{
    public class MockCommandExecutor : ICommandExecutor
    {
        private readonly ILogger<MockCommandExecutor> _logger;

        public MockCommandExecutor(ILogger<MockCommandExecutor> logger)
        {
            _logger = logger;
        }

        public Task ExecuteAsync(DeviceCommand command, string batteryId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[MockExecutor] Executing '{Cmd}' for battery {Battery}", command.Command, batteryId);
            // simulate short execution
            return Task.Delay(200, cancellationToken);
        }
    }
}

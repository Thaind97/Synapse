using System.Threading;
using System.Threading.Tasks;
using Synapse.Services.Services.Abstraction;
using Synapse.Services.Services.Abstraction;
using Synapse.Infrastructure.Entities;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Synapse.Services.Services.Implement
{
    public class SequenceRunner : ISequenceRunner
    {
        private readonly ISequenceService _sequenceService;
        private readonly ICommandExecutor _commandExecutor;
        private readonly ILogger<SequenceRunner> _logger;

        public SequenceRunner(ISequenceService sequenceService, ICommandExecutor commandExecutor, ILogger<SequenceRunner> logger)
        {
            _sequenceService = sequenceService;
            _commandExecutor = commandExecutor;
            _logger = logger;
        }

        public async Task RunAsync(long sequenceId, string batteryId, CancellationToken cancellationToken = default)
        {
            // Load sequence with steps & commands
            var sequence = await _sequenceService.GetSequenceByIdAsync(sequenceId);
            if (sequence == null)
            {
                _logger.LogWarning("Sequence {Id} not found", sequenceId);
                return;
            }

            _logger.LogInformation("Run sequence {Id} for battery {Battery}", sequenceId, batteryId);

            var sortedSteps = sequence.Steps.OrderBy(s => s.StepOrder).ToList();
            foreach (var step in sortedSteps)
            {
                if (cancellationToken.IsCancellationRequested) break;

                _logger.LogInformation("Running step {Step} ({Order})", step.StepName, step.StepOrder);

                var commands = step.DeviceCommands?.Where(c => c.ParentCommandId == null).ToList() ?? new List<DeviceCommand>();
                await ExecuteCommands(commands, step, batteryId, cancellationToken);
            }

            _logger.LogInformation("Finished run {Id} for battery {Battery}", sequenceId, batteryId);
        }

        private async Task ExecuteCommands(List<DeviceCommand> commands, SequenceStep step, string batteryId, CancellationToken ct)
        {
            foreach (var cmd in commands)
            {
                if (ct.IsCancellationRequested) return;

                if (string.Equals(cmd.Command, "LOOP", StringComparison.OrdinalIgnoreCase))
                {
                    var loopCountParam = cmd.CommandParameters?.FirstOrDefault(p => p.Name == "Loop Count");
                    int count = 1;
                    if (loopCountParam != null && int.TryParse(loopCountParam.Value, out var v)) count = v;

                    var children = step.DeviceCommands?.Where(c => c.ParentCommandId == cmd.Id).ToList() ?? new List<DeviceCommand>();
                    for (int i = 0; i < count; i++)
                    {
                        if (ct.IsCancellationRequested) return;
                        _logger.LogInformation("Loop {Id} iteration {Iteration}", cmd.Id, i+1);
                        await ExecuteCommands(children, step, batteryId, ct);
                    }
                }
                else if (string.Equals(cmd.Command, "Wait", StringComparison.OrdinalIgnoreCase) || cmd.Device == "Wait")
                {
                    var waitParam = cmd.CommandParameters?.FirstOrDefault(p => p.Name == "Wait Time");
                    var waitSeconds = 10;
                    if (waitParam != null && int.TryParse(waitParam.Value, out var w)) waitSeconds = w;
                    _logger.LogInformation("Waiting {Seconds}s", waitSeconds);
                    try { await Task.Delay(TimeSpan.FromSeconds(waitSeconds), ct); } catch (OperationCanceledException) { return; }
                }
                else
                {
                    _logger.LogInformation("Executing command {Cmd} on device {Dev}", cmd.Command, cmd.Device);
                    await _commandExecutor.ExecuteAsync(cmd, batteryId, ct);
                }
            }
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Synapse.Services.Services.Abstraction;

namespace Synapse.Services.Services.Implement
{
    public class RunManager : IRunManager, IDisposable
    {
        private readonly ISequenceRunner _runner;
        private readonly ILogger<RunManager> _logger;

        // per-battery channel and worker
        private readonly ConcurrentDictionary<string, Channel<RunRequest>> _queues = new();
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _runningCts = new();
        private readonly ConcurrentDictionary<string, Task> _workers = new();
        private readonly ConcurrentDictionary<string, string> _statuses = new();

        public RunManager(ISequenceRunner runner, ILogger<RunManager> logger)
        {
            _runner = runner;
            _logger = logger;
        }

        public async Task EnqueueRunAsync(long sequenceId, string batteryId)
        {
            if (string.IsNullOrWhiteSpace(batteryId))
            {
                _logger.LogWarning("EnqueueRunAsync called with empty batteryId -- ignoring");
                return;
            }

            batteryId = batteryId.Trim();

            var ch = _queues.GetOrAdd(batteryId, id => CreateChannelAndStartWorker(id));
            var req = new RunRequest { SequenceId = sequenceId, BatteryId = batteryId };
            await ch.Writer.WriteAsync(req);
            _statuses[batteryId] = "Queued";
        }

        public Task StopRunAsync(string batteryId)
        {
            if (string.IsNullOrWhiteSpace(batteryId))
            {
                _logger.LogWarning("StopRunAsync called with empty batteryId -- ignoring");
                return Task.CompletedTask;
            }

            batteryId = batteryId.Trim();

            if (_runningCts.TryGetValue(batteryId, out var cts))
            {
                cts.Cancel();
                _statuses[batteryId] = "Stopping";
            }
            else
            {
                _logger.LogInformation("No running job found for battery {Battery}", batteryId);
            }

            return Task.CompletedTask;
        }

        public IReadOnlyDictionary<string, string> GetStatuses() => _statuses;

        private Channel<RunRequest> CreateChannelAndStartWorker(string batteryId)
        {
            var ch = Channel.CreateUnbounded<RunRequest>();
            var workerTask = Task.Run(async () => await WorkerLoop(batteryId, ch.Reader));
            _workers[batteryId] = workerTask;
            return ch;
        }

        private async Task WorkerLoop(string batteryId, ChannelReader<RunRequest> reader)
        {
            await foreach (var req in reader.ReadAllAsync())
            {
                _statuses[batteryId] = "Running";
                var cts = new CancellationTokenSource();
                _runningCts[batteryId] = cts;
                try
                {
                    _logger.LogInformation("Worker for {Battery} running sequence {Seq}", batteryId, req.SequenceId);
                    await _runner.RunAsync(req.SequenceId, batteryId, cts.Token);
                    _statuses[batteryId] = "Idle";
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Run canceled for {Battery}", batteryId);
                    _statuses[batteryId] = "Canceled";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error running sequence for {Battery}", batteryId);
                    _statuses[batteryId] = "Error";
                }
                finally
                {
                    _runningCts.TryRemove(batteryId, out _);
                }
            }
        }

        public void Dispose()
        {
            foreach (var c in _runningCts.Values)
                c.Cancel();
        }

        private class RunRequest
        {
            public long SequenceId { get; set; }
            public string BatteryId { get; set; } = string.Empty;
        }
    }
}

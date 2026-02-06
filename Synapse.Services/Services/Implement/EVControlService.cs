using Synapse.Service.Tasking.HttpClients;
using Synapse.Services.Models;
using Synapse.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Synapse.Services
{
    /// <summary>
    /// EV Control Service implementation that fetches data from API
    /// Replace the mock implementation with actual API calls when ready
    /// </summary>
    public class EVControlService : IEVControlService
    {
        private readonly TaskingHttpObject _httpClient;
        private bool _isRunning;
        private bool _isPaused;
        private readonly Random _random = new();

        public EVControlService(TaskingHttpObject httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<EVControlData> GetInitialDataAsync()
        {
            // TODO: Replace with actual API call
            // var response = await _httpClient.GetAsync<EVControlData>("api/evcontrol/initial");
            
            await Task.Delay(50); // Simulate network delay

            var batteries = new List<BatteryData>();
            for (int i = 1; i <= 16; i++)
            {
                var status = BatteryStatus.Normal;
                if (i == 4) status = BatteryStatus.Warning;
                if (i == 5 || i == 13 || i == 15) status = BatteryStatus.Inactive;

                var voltageHistory = new List<double>();
                var currentHistory = new List<double>();
                var random = new Random(i);
                for (int j = 0; j < 10; j++)
                {
                    voltageHistory.Add(3.5 + random.NextDouble() * 0.7);
                    currentHistory.Add(0.5 + random.NextDouble() * 0.5);
                }

                batteries.Add(new BatteryData
                {
                    Id = i,
                    Name = $"Battery {i}",
                    Voltage = 10 + (i % 10) + (i * 0.5),
                    Current = 5.0,
                    Temperature = 20.0,
                    StateOfCharge = 70 + (i % 20),
                    PassCount = 0,
                    Status = status,
                    VoltageHistory = voltageHistory,
                    CurrentHistory = currentHistory
                });
            }

            var steps = new List<StepData>
            {
                new() { Step = "Charge to 80%", Description = "CC-CV charging", Status = StepStatus.Completed },
                new() { Step = "Wait for 10min", Description = "Rest period", Status = StepStatus.Active },
                new() { Step = "Discharge", Description = "CC discharge", Status = StepStatus.Pending },
                new() { Step = "Wait for 10min", Description = "Rest period", Status = StepStatus.Pending }
            };

            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var logEntries = new List<LogEntryData>
            {
                new() { Timestamp = $"[{timestamp}]", Message = "INFO: Experiment \"Test battery cells\" started", Level = LogLevel.Info },
                new() { Timestamp = $"[{timestamp}]", Message = "INFO: Pattern \"CCCV Charge\" started", Level = LogLevel.Info },
                new() { Timestamp = $"[{timestamp}]", Message = "INFO: Pattern \"CCCV Charge\" started", Level = LogLevel.Info },
                new() { Timestamp = $"[{timestamp}]", Message = "ERROR: Emergency stop because voltage > 5", Level = LogLevel.Error }
            };

            return new EVControlData
            {
                Batteries = batteries,
                Steps = steps,
                LogEntries = logEntries,
                PatternName = "CC Charge"
            };
        }

        public async Task<List<BatteryData>> GetBatteryDataAsync()
        {
            // TODO: Replace with actual API call
            // var response = await _httpClient.GetAsync<List<BatteryData>>("api/evcontrol/batteries");
            
            await Task.Delay(10); // Simulate minimal network delay

            var batteries = new List<BatteryData>();
            for (int i = 1; i <= 16; i++)
            {
                batteries.Add(new BatteryData
                {
                    Id = i,
                    Name = $"Battery {i}",
                    Voltage = 3.0 + (_random.NextDouble() * 1.5),
                    Current = 0.3 + (_random.NextDouble() * 1.2),
                    Temperature = 20.0 + (_random.NextDouble() * 5.0),
                    StateOfCharge = 70 + _random.Next(-5, 6),
                    PassCount = _random.Next(0, 10)
                });
            }

            return batteries;
        }

        public async Task<BatteryData?> GetBatteryDataAsync(int batteryId)
        {
            // TODO: Replace with actual API call
            // var response = await _httpClient.GetAsync<BatteryData>($"api/evcontrol/batteries/{batteryId}");
            
            await Task.Delay(5);

            return new BatteryData
            {
                Id = batteryId,
                Name = $"Battery {batteryId}",
                Voltage = 3.0 + (_random.NextDouble() * 1.5),
                Current = 0.3 + (_random.NextDouble() * 1.2),
                Temperature = 20.0 + (_random.NextDouble() * 5.0),
                StateOfCharge = 70 + _random.Next(-5, 6),
                PassCount = _random.Next(0, 10)
            };
        }

        public async Task<bool> StartExperimentAsync(string patternName)
        {
            // TODO: Replace with actual API call
            // var response = await _httpClient.PostAsync<bool>("api/evcontrol/start", new { PatternName = patternName });
            
            await Task.Delay(100);
            _isRunning = true;
            _isPaused = false;
            return true;
        }

        public async Task<bool> StopExperimentAsync()
        {
            // TODO: Replace with actual API call
            // var response = await _httpClient.PostAsync<bool>("api/evcontrol/stop", null);
            
            await Task.Delay(100);
            _isRunning = false;
            _isPaused = false;
            return true;
        }

        public async Task<bool> TogglePauseAsync()
        {
            // TODO: Replace with actual API call
            // var response = await _httpClient.PostAsync<bool>("api/evcontrol/toggle-pause", null);
            
            await Task.Delay(50);
            _isPaused = !_isPaused;
            return _isPaused;
        }

        public async Task<List<LogEntryData>> GetLogEntriesAsync(int count = 100)
        {
            // TODO: Replace with actual API call
            // var response = await _httpClient.GetAsync<List<LogEntryData>>($"api/evcontrol/logs?count={count}");
            
            await Task.Delay(20);
            
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            return new List<LogEntryData>
            {
                new() { Timestamp = $"[{timestamp}]", Message = "INFO: System running normally", Level = LogLevel.Info }
            };
        }
    }
}

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

        public async Task<List<BatteryData>> GetBatteryDataAsync()
        {
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
                    PassCount = _random.Next(0, 10),
                    AmpereHour = 5.0 + (_random.NextDouble() * 1.0),
                    Capacity = 5.0 + (_random.NextDouble() * 1.0)
                });
            }

            return batteries;
        }

        public async Task<bool> StartExperimentAsync(string patternName)
        {
            await Task.Delay(100);
            _isRunning = true;
            _isPaused = false;
            return true;
        }

        public async Task<bool> StopExperimentAsync()
        {
            await Task.Delay(100);
            _isRunning = false;
            _isPaused = false;
            return true;
        }

        public async Task<bool> TogglePauseAsync()
        {
            await Task.Delay(50);
            _isPaused = !_isPaused;
            return _isPaused;
        }
    }
}

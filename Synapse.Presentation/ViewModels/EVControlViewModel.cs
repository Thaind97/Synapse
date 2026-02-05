using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Synapse.Presentation.Models;
using Synapse.Presentation.Services;

namespace Synapse.Presentation.ViewModels
{
    public class EVControlViewModel : ObservableObject, IDisposable
    {
        private bool _isRunning;
        private CancellationTokenSource? _cts;
        private BatteryInfoModel? _selectedBattery;
        private readonly IpcManager _ipcManager;
        private Timer? _uiUpdateTimer;

        public ObservableCollection<BatteryInfoModel> Batteries { get; set; } = new();
        public ObservableCollection<StepItemModel> Steps { get; set; } = new();
        public SeriesCollection OverviewSeries { get; set; } = new();
        public string[] OverviewLabels { get; set; } = Array.Empty<string>();

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand SelectBatteryCommand { get; }

        public bool IsRunning
        {
            get => _isRunning;
            private set
            {
                if (SetProperty(ref _isRunning, value))
                {
                    OnPropertyChanged(nameof(StatusDescription));
                }
            }
        }

        public string StatusDescription => IsRunning ? "SYSTEM RUNNING - REALTIME IPC (SHARED MEMORY)" : "SYSTEM IDLE";

        public BatteryInfoModel? SelectedBattery
        {
            get => _selectedBattery;
            set
            {
                if (_selectedBattery != null) _selectedBattery.IsSelected = false;
                if (SetProperty(ref _selectedBattery, value))
                {
                    if (_selectedBattery != null) _selectedBattery.IsSelected = true;
                    UpdateOverviewChart();
                    OnPropertyChanged(nameof(SelectedBatteryName));
                }
            }
        }

        public string SelectedBatteryName => SelectedBattery?.Name ?? "No Selection";

        public EVControlViewModel()
        {
            _ipcManager = new IpcManager();
            
            StartCommand = new RelayCommand(StartRealtime);
            StopCommand = new RelayCommand(StopRealtime);
            SelectBatteryCommand = new RelayCommand<BatteryInfoModel>(battery => SelectedBattery = battery);

            InitializeView();
        }

        private void InitializeView()
        {
            // Initialize 24 batteries
            for (int i = 1; i <= 24; i++)
            {
                var id = i.ToString("D2");
                Batteries.Add(new BatteryInfoModel
                {
                    Name = $"Battery {id}",
                    Voltage = 0,
                    StateOfCharge = 0,
                    PassCount = 0,
                    HistoryData = new ChartValues<double>()
                });
            }

            SelectedBattery = Batteries.FirstOrDefault();

            Steps.Add(new StepItemModel { Step = "IPC Warmup", Description = "Initializing Shared Memory segments..." });
            Steps.Add(new StepItemModel { Step = "Service Sync", Description = "Broadcasting thread handles to IPC manager" });
            Steps.Add(new StepItemModel { Step = "Realtime Data", Description = "Polling MMViewAccessor at 50ms interval" });
            Steps.Add(new StepItemModel { Step = "Logging", Description = "Writing IPC transactions to binlog" });

            OverviewLabels = Enumerable.Range(0, 20).Select(i => i.ToString()).ToArray();
        }

        private void UpdateOverviewChart()
        {
            if (SelectedBattery == null) return;

            OverviewSeries.Clear();
            OverviewSeries.Add(new LineSeries
            {
                Title = "IPC Voltage Data",
                Values = SelectedBattery.HistoryData,
                PointGeometry = null,
                Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(59, 130, 246)),
                Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(20, 59, 130, 246))
            });
        }

        private void StartRealtime()
        {
            if (IsRunning) return;

            IsRunning = true;
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            // 1. Start 24 "Service Threads" that write to Shared Memory
            for (int i = 0; i < 24; i++)
            {
                int threadIndex = i;
                Task.Run(() => RunExternalServiceSimulation(threadIndex, token), token);
            }

            // 2. Start UI Refresh Timer (Reading from Shared Memory)
            // This decouples data acquisition from UI update
            _uiUpdateTimer = new Timer(UpdateUiFromSharedMemory, null, 100, 100);
        }

        private void StopRealtime()
        {
            if (!IsRunning) return;

            _cts?.Cancel();
            _uiUpdateTimer?.Dispose();
            _uiUpdateTimer = null;
            IsRunning = false;
        }

        /// <summary>
        /// Simulates an external process or service writing to Shared Memory
        /// </summary>
        private async Task RunExternalServiceSimulation(int index, CancellationToken token)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            double voltage = 12.0;
            int soc = 70;
            int pass = 0;

            while (!token.IsCancellationRequested)
            {
                // Simulate processing/acquisition time
                await Task.Delay(50 + random.Next(1, 100), token);

                voltage = 11.5 + (random.NextDouble() * 1.5);
                soc = Math.Clamp(soc + random.Next(-1, 2), 0, 100);
                if (random.Next(1, 100) > 98) pass++;

                // WRITE to Shared Memory (IPC)
                _ipcManager.WriteData(index, new BatterySharedData
                {
                    Voltage = voltage,
                    StateOfCharge = soc,
                    PassCount = pass,
                    LastUpdateTick = Environment.TickCount
                });
            }
        }

        /// <summary>
        /// Periodic task to READ from Shared Memory and update UI
        /// </summary>
        private void UpdateUiFromSharedMemory(object? state)
        {
            if (!IsRunning) return;

            // Use Dispatcher to update ObservableCollection/ViewProperties
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < Batteries.Count; i++)
                {
                    // READ from Shared Memory (IPC)
                    var data = _ipcManager.ReadData(i);
                    var battery = Batteries[i];

                    battery.Voltage = data.Voltage;
                    battery.StateOfCharge = data.StateOfCharge;
                    battery.PassCount = data.PassCount;

                    // Update History for individual charts
                    battery.HistoryData.Add(data.Voltage);
                    if (battery.HistoryData.Count > 20) battery.HistoryData.RemoveAt(0);
                }
            });
        }

        public void Dispose()
        {
            _uiUpdateTimer?.Dispose();
            _ipcManager.Dispose();
        }
    }
}

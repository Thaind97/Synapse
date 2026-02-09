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
using Synapse.Services;
using Synapse.Services.Models;
using Synapse.Shared.Enums;
using Synapse.Shared.Helper;

namespace Synapse.Presentation.ViewModels
{
    public class EVControlViewModel : ObservableObject, IDisposable
    {
        private bool _isRunning;
        private bool _isPaused;
        private CancellationTokenSource? _cts;
        private BatteryInfo? _selectedBattery;
        private readonly IpcManager _ipcManager;
        private readonly IEVControlService? _evControlService;
        private Timer? _uiUpdateTimer;
        private Timer? _elapsedTimer;
        private DateTime _startTime;
        private string _searchText = string.Empty;
        private string _elapsedTime = "00:00:00";
        private string _patternElapsedTime = "00:00:00";
        private string _patternName = "CC Charge";
        private int _currentStepIndex = 0;
        private bool _useRealData = true;
        private BatteryStatus? _statusFilter = null;

        public ObservableCollection<BatteryInfo> Batteries { get; set; } = new();
        public ObservableCollection<StepItem> Steps { get; set; } = new();
        public ObservableCollection<LogEntry> LogEntries { get; set; } = new();
        public SeriesCollection OverviewSeries { get; set; } = new();
        public SeriesCollection CurrentSeries { get; set; } = new();
        public string[] OverviewLabels { get; set; } = Array.Empty<string>();

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand PauseResumeCommand { get; }
        public ICommand SelectBatteryCommand { get; }
        public ICommand FilterCommand { get; }

        /// <summary>
        /// Filtered batteries based on search text and status filter
        /// </summary>
        public IEnumerable<BatteryInfo> FilteredBatteries
        {
            get
            {
                var filtered = Batteries.AsEnumerable();

                // Filter by search text (channel name)
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    filtered = filtered.Where(b => b.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
                }

                // Filter by status
                if (_statusFilter.HasValue)
                {
                    filtered = filtered.Where(b => b.Status == _statusFilter.Value);
                }

                return filtered;
            }
        }

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

        public bool IsPaused
        {
            get => _isPaused;
            private set
            {
                if (SetProperty(ref _isPaused, value))
                {
                    OnPropertyChanged(nameof(PauseResumeText));
                }
            }
        }

        public string PauseResumeText => IsPaused ? "Resume" : "Pause/Resume";

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    OnPropertyChanged(nameof(FilteredBatteries));
                }
            }
        }

        public string ElapsedTime
        {
            get => _elapsedTime;
            set => SetProperty(ref _elapsedTime, value);
        }

        public string PatternElapsedTime
        {
            get => _patternElapsedTime;
            set => SetProperty(ref _patternElapsedTime, value);
        }

        public string PatternName
        {
            get => _patternName;
            set => SetProperty(ref _patternName, value);
        }

        public int CurrentStepIndex
        {
            get => _currentStepIndex;
            set => SetProperty(ref _currentStepIndex, value);
        }

        public string StatusDescription => IsRunning ? "SYSTEM RUNNING - REALTIME DATA" : "SYSTEM IDLE";

        public BatteryInfo? SelectedBattery
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
            
            // Try to resolve service from DI container
            _evControlService = ServiceHelper.GetService<IEVControlService>();
            _useRealData = _evControlService != null;
            
            StartCommand = new RelayCommand(StartRealtime);
            StopCommand = new RelayCommand(StopRealtime);
            PauseResumeCommand = new RelayCommand(TogglePauseResume);
            SelectBatteryCommand = new RelayCommand<BatteryInfo>(battery => SelectedBattery = battery);
            FilterCommand = new RelayCommand(ShowFilterOptions);

            // Initialize asynchronously
            _ = InitializeViewAsync();
        }

        public EVControlViewModel(IEVControlService evControlService)
        {
            _ipcManager = new IpcManager();
            _evControlService = evControlService;
            _useRealData = true;
            
            StartCommand = new RelayCommand(StartRealtime);
            StopCommand = new RelayCommand(StopRealtime);
            PauseResumeCommand = new RelayCommand(TogglePauseResume);
            SelectBatteryCommand = new RelayCommand<BatteryInfo>(battery => SelectedBattery = battery);
            FilterCommand = new RelayCommand(ShowFilterOptions);

            // Initialize asynchronously
            _ = InitializeViewAsync();
        }

        /// <summary>
        /// Shows filter options dialog or cycles through status filters
        /// </summary>
        private void ShowFilterOptions()
        {
            // Cycle through filter options: All -> Normal -> Warning -> Inactive -> All
            if (_statusFilter == null)
            {
                _statusFilter = BatteryStatus.Normal;
                AddLogEntry("Filter: Showing Normal channels", LogLevel.Info);
            }
            else if (_statusFilter == BatteryStatus.Normal)
            {
                _statusFilter = BatteryStatus.Warning;
                AddLogEntry("Filter: Showing Warning channels", LogLevel.Info);
            }
            else if (_statusFilter == BatteryStatus.Warning)
            {
                _statusFilter = BatteryStatus.Inactive;
                AddLogEntry("Filter: Showing Inactive channels", LogLevel.Info);
            }
            else
            {
                _statusFilter = null;
                AddLogEntry("Filter: Showing All channels", LogLevel.Info);
            }

            OnPropertyChanged(nameof(FilteredBatteries));
        }

        /// <summary>
        /// Sets the status filter directly
        /// </summary>
        public void SetStatusFilter(BatteryStatus? status)
        {
            _statusFilter = status;
            OnPropertyChanged(nameof(FilteredBatteries));
        }

        /// <summary>
        /// Clears all filters
        /// </summary>
        public void ClearFilters()
        {
            SearchText = string.Empty;
            _statusFilter = null;
            OnPropertyChanged(nameof(FilteredBatteries));
        }

        private async Task InitializeViewAsync()
        {
            if (_useRealData && _evControlService != null)
            {
                await LoadDataFromServiceAsync();
            }
            else
            {
                InitializeWithMockData();
            }
        }

        private async Task LoadDataFromServiceAsync()
        {
            try
            {
                var data = await _evControlService!.GetInitialDataAsync();

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    // Load batteries
                    Batteries.Clear();
                    foreach (var batteryData in data.Batteries)
                    {
                        var historyData = new ChartValues<double>(batteryData.VoltageHistory);
                        var currentHistoryData = new ChartValues<double>(batteryData.CurrentHistory);

                        Batteries.Add(new BatteryInfo
                        {
                            Name = batteryData.Name,
                            Voltage = batteryData.Voltage,
                            Current = batteryData.Current,
                            Temperature = batteryData.Temperature,
                            StateOfCharge = batteryData.StateOfCharge,
                            PassCount = batteryData.PassCount,
                            AmpereHour = batteryData.AmpereHour,
                            Capacity = batteryData.Capacity,
                            Status = batteryData.Status,
                            HistoryData = historyData,
                            CurrentHistoryData = currentHistoryData
                        });
                    }

                    SelectedBattery = Batteries.FirstOrDefault();
                    OnPropertyChanged(nameof(FilteredBatteries));

                    // Load steps
                    Steps.Clear();
                    foreach (var stepData in data.Steps)
                    {
                        Steps.Add(new StepItem
                        {
                            Step = stepData.Step,
                            Description = stepData.Description,
                            Status = stepData.Status
                        });
                    }

                    // Load log entries
                    LogEntries.Clear();
                    foreach (var logData in data.LogEntries)
                    {
                        LogEntries.Add(new LogEntry
                        {
                            Timestamp = logData.Timestamp,
                            Message = logData.Message,
                            Level = logData.Level
                        });
                    }

                    PatternName = data.PatternName;
                    OverviewLabels = Enumerable.Range(0, 65).Where(x => x % 5 == 0).Select(i => i.ToString()).ToArray();

                    InitializeOverviewChart();
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data from service: {ex.Message}");
                // Fallback to mock data
                InitializeWithMockData();
            }
        }

        private void InitializeWithMockData()
        {
            // Initialize 16 batteries (matching screenshot)
            for (int i = 1; i <= 16; i++)
            {
                var status = BatteryStatus.Normal;
                // Set different statuses for demo (matching screenshot)
                if (i == 4) status = BatteryStatus.Warning;
                if (i == 5 || i == 13 || i == 15) status = BatteryStatus.Inactive;

                var historyData = new ChartValues<double>();
                var currentHistoryData = new ChartValues<double>();
                
                // Add initial data points to prevent empty chart issues
                var random = new Random(i);
                for (int j = 0; j < 10; j++)
                {
                    historyData.Add(3.5 + random.NextDouble() * 0.7);
                    currentHistoryData.Add(0.5 + random.NextDouble() * 0.5);
                }

                Batteries.Add(new BatteryInfo
                {
                    Name = $"Battery {i}",
                    Voltage = 10 + (i % 10) + (i * 0.5),
                    Current = 5.0,
                    Temperature = 20.0,
                    StateOfCharge = 70 + (i % 20),
                    PassCount = 0,
                    AmpereHour = 5.6,
                    Capacity = 5.6,
                    Status = status,
                    HistoryData = historyData,
                    CurrentHistoryData = currentHistoryData
                });
            }

            SelectedBattery = Batteries.FirstOrDefault();
            OnPropertyChanged(nameof(FilteredBatteries));

            // Initialize Steps (matching screenshot)
            Steps.Add(new StepItem { Step = "Charge to 80%", Description = "CC-CV charging", Status = StepStatus.Completed });
            Steps.Add(new StepItem { Step = "Wait for 10min", Description = "Rest period", Status = StepStatus.Active });
            Steps.Add(new StepItem { Step = "Discharge", Description = "CC discharge", Status = StepStatus.Pending });
            Steps.Add(new StepItem { Step = "Wait for 10min", Description = "Rest period", Status = StepStatus.Pending });

            // Initialize Log Entries (matching screenshot)
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            LogEntries.Add(new LogEntry { Timestamp = $"[{timestamp}]", Message = "INFO: Experiment \"Test battery cells\" started", Level = LogLevel.Info });
            LogEntries.Add(new LogEntry { Timestamp = $"[{timestamp}]", Message = "INFO: Pattern \"CCCV Charge\" started", Level = LogLevel.Info });
            LogEntries.Add(new LogEntry { Timestamp = $"[{timestamp}]", Message = "INFO: Pattern \"CCCV Charge\" started", Level = LogLevel.Info });
            LogEntries.Add(new LogEntry { Timestamp = $"[{timestamp}]", Message = "ERROR: Emergency stop because voltage > 5", Level = LogLevel.Error });

            OverviewLabels = Enumerable.Range(0, 65).Where(x => x % 5 == 0).Select(i => i.ToString()).ToArray();

            // Initialize OverviewSeries with empty series to prevent null reference
            InitializeOverviewChart();
        }

        private void InitializeOverviewChart()
        {
            OverviewSeries.Clear();
            
            // Initialize with data from selected battery if available
            var voltageValues = SelectedBattery?.HistoryData ?? new ChartValues<double>();
            var currentValues = SelectedBattery?.CurrentHistoryData ?? new ChartValues<double>();
            
            // Voltage series - Blue line (left Y-axis, index 0)
            OverviewSeries.Add(new LineSeries
            {
                Title = "Voltage",
                Values = voltageValues,
                PointGeometry = null,
                Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(59, 130, 246)),
                Fill = System.Windows.Media.Brushes.Transparent,
                LineSmoothness = 0.5,
                ScalesYAt = 0
            });

            // Current series - Orange line (right Y-axis, index 1)
            OverviewSeries.Add(new LineSeries
            {
                Title = "Current",
                Values = currentValues,
                PointGeometry = null,
                Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(249, 115, 22)),
                Fill = System.Windows.Media.Brushes.Transparent,
                LineSmoothness = 0.5,
                ScalesYAt = 1
            });
        }

        private void UpdateOverviewChart()
        {
            if (SelectedBattery == null) return;

            try
            {
                // Update the Values of existing series instead of recreating them
                if (OverviewSeries.Count >= 2)
                {
                    ((LineSeries)OverviewSeries[0]).Values = SelectedBattery.HistoryData;
                    ((LineSeries)OverviewSeries[1]).Values = SelectedBattery.CurrentHistoryData;
                }
            }
            catch
            {
                // Ignore errors during chart update
            }
        }

        private void TogglePauseResume()
        {
            if (!IsRunning) return;
            IsPaused = !IsPaused;
            
            if (IsPaused)
            {
                AddLogEntry("INFO: System paused", LogLevel.Info);
            }
            else
            {
                AddLogEntry("INFO: System resumed", LogLevel.Info);
            }

            // Notify service if available
            if (_evControlService != null)
            {
                _ = _evControlService.TogglePauseAsync();
            }
        }

        private void AddLogEntry(string message, LogLevel level)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                LogEntries.Add(new LogEntry { Timestamp = $"[{timestamp}]", Message = message, Level = level });
            });
        }

        private async void StartRealtime()
        {
            if (IsRunning) return;

            IsRunning = true;
            IsPaused = false;
            _startTime = DateTime.Now;
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            AddLogEntry("INFO: Experiment started", LogLevel.Info);
            AddLogEntry($"INFO: Pattern \"{PatternName}\" started", LogLevel.Info);

            // Notify service if available
            if (_evControlService != null)
            {
                await _evControlService.StartExperimentAsync(PatternName);
            }

            // Update step status
            if (Steps.Count > 0)
            {
                Steps[0].Status = StepStatus.Active;
            }

            if (_useRealData && _evControlService != null)
            {
                // Start real-time data polling from service
                _uiUpdateTimer = new Timer(UpdateUiFromService, null, 100, 100);
            }
            else
            {
                // Start simulation threads for mock data
                for (int i = 0; i < Batteries.Count; i++)
                {
                    int threadIndex = i;
                    Task.Run(() => RunExternalServiceSimulation(threadIndex, token), token);
                }
                _uiUpdateTimer = new Timer(UpdateUiFromSharedMemory, null, 100, 100);
            }

            // Start Elapsed Time Timer
            _elapsedTimer = new Timer(UpdateElapsedTime, null, 1000, 1000);
        }

        private void UpdateElapsedTime(object? state)
        {
            var elapsed = DateTime.Now - _startTime;
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ElapsedTime = elapsed.ToString(@"hh\:mm\:ss");
                PatternElapsedTime = elapsed.ToString(@"hh\:mm\:ss");
            });
        }

        private async void StopRealtime()
        {
            if (!IsRunning) return;

            _cts?.Cancel();
            _uiUpdateTimer?.Dispose();
            _uiUpdateTimer = null;
            _elapsedTimer?.Dispose();
            _elapsedTimer = null;
            IsRunning = false;
            IsPaused = false;

            // Notify service if available
            if (_evControlService != null)
            {
                await _evControlService.StopExperimentAsync();
            }

            AddLogEntry("INFO: Experiment stopped", LogLevel.Info);
        }

        /// <summary>
        /// Updates UI from the EV Control service (real data)
        /// </summary>
        private async void UpdateUiFromService(object? state)
        {
            if (!IsRunning || IsPaused || _evControlService == null) return;

            try
            {
                var batteryDataList = await _evControlService.GetBatteryDataAsync();

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    for (int i = 0; i < Math.Min(Batteries.Count, batteryDataList.Count); i++)
                    {
                        var data = batteryDataList[i];
                        var battery = Batteries[i];

                        battery.Voltage = data.Voltage;
                        battery.Current = data.Current;
                        battery.Temperature = data.Temperature;
                        battery.StateOfCharge = data.StateOfCharge;
                        battery.PassCount = data.PassCount;
                        battery.AmpereHour = data.AmpereHour;
                        battery.Capacity = data.Capacity;

                        // Update History for individual charts
                        battery.HistoryData.Add(data.Voltage);
                        if (battery.HistoryData.Count > 65) battery.HistoryData.RemoveAt(0);

                        battery.CurrentHistoryData.Add(data.Current);
                        if (battery.CurrentHistoryData.Count > 65) battery.CurrentHistoryData.RemoveAt(0);
                    }

                    // Refresh filtered view
                    OnPropertyChanged(nameof(FilteredBatteries));
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating from service: {ex.Message}");
            }
        }

        /// <summary>
        /// Simulates an external process or service writing to Shared Memory (fallback)
        /// </summary>
        private async Task RunExternalServiceSimulation(int index, CancellationToken token)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            double voltage = 3.5 + random.NextDouble();
            double current = 0.5 + random.NextDouble();
            int soc = 70;
            int pass = 0;

            while (!token.IsCancellationRequested)
            {
                // Simulate processing/acquisition time
                await Task.Delay(50 + random.Next(1, 100), token);

                if (!IsPaused)
                {
                    voltage = 3.0 + (random.NextDouble() * 1.5);
                    current = 0.3 + (random.NextDouble() * 1.2);
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
        }

        /// <summary>
        /// Periodic task to READ from Shared Memory and update UI (fallback)
        /// </summary>
        private void UpdateUiFromSharedMemory(object? state)
        {
            if (!IsRunning || IsPaused) return;

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
                    battery.Current = 0.5 + (new Random().NextDouble() * 1.0);
                    battery.Temperature = 20.0 + (new Random().NextDouble() * 5.0);

                    // Update History for individual charts
                    battery.HistoryData.Add(data.Voltage);
                    if (battery.HistoryData.Count > 65) battery.HistoryData.RemoveAt(0);

                    battery.CurrentHistoryData.Add(battery.Current);
                    if (battery.CurrentHistoryData.Count > 65) battery.CurrentHistoryData.RemoveAt(0);
                }

                // Refresh filtered view
                OnPropertyChanged(nameof(FilteredBatteries));
            });
        }

        public void Dispose()
        {
            _uiUpdateTimer?.Dispose();
            _elapsedTimer?.Dispose();
            _ipcManager.Dispose();
        }
    }
}

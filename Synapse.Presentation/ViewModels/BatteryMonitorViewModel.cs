using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveCharts;
using LiveCharts.Wpf;
using Synapse.Presentation.Models;
using Synapse.Shared.Enums;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;

namespace Synapse.Presentation.ViewModels
{
    /// <summary>
    /// ViewModel for the Battery Monitoring Dashboard
    /// Displays a grid of battery cells, charts, measurements, step table, and logs
    /// </summary>
    public class BatteryMonitorViewModel : ObservableObject, IDisposable
    {
        private bool _isRunning;
        private bool _isPaused;
        private CancellationTokenSource? _cts;
        private Timer? _updateTimer;
        private BatteryCellModel? _selectedBattery;
        private string _searchText = string.Empty;
        private TimeSpan _stepTableElapsed;

        #region Collections

        /// <summary>
        /// Collection of all battery cells (16 cells as shown in the screenshot)
        /// </summary>
        public ObservableCollection<BatteryCellModel> BatteryCells { get; } = new();

        /// <summary>
        /// Measurement values panel (Voltage, Current, Temperature, Ampere Hour, Capacity)
        /// </summary>
        public ObservableCollection<MeasurementValueModel> MeasurementValues { get; } = new();

        /// <summary>
        /// Step table items (Charge, Wait, Discharge, etc.)
        /// </summary>
        public ObservableCollection<StepTableItemModel> StepTableItems { get; } = new();

        /// <summary>
        /// System log entries
        /// </summary>
        public ObservableCollection<SystemLogModel> SystemLogs { get; } = new();

        #endregion

        #region Chart Properties

        /// <summary>
        /// Chart series for the selected battery (Voltage and Current)
        /// </summary>
        public SeriesCollection ChartSeries { get; set; } = new();

        /// <summary>
        /// X-axis labels for the chart (Time in seconds)
        /// </summary>
        public string[] ChartLabels { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Y-axis formatter for voltage
        /// </summary>
        public Func<double, string> VoltageFormatter { get; } = value => $"{value:F1}";

        /// <summary>
        /// Y-axis formatter for current
        /// </summary>
        public Func<double, string> CurrentFormatter { get; } = value => $"{value:F1}";

        #endregion

        #region Observable Properties

        public bool IsRunning
        {
            get => _isRunning;
            private set
            {
                if (SetProperty(ref _isRunning, value))
                {
                    OnPropertyChanged(nameof(CanStart));
                    OnPropertyChanged(nameof(CanStop));
                    OnPropertyChanged(nameof(CanPauseResume));
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
                    OnPropertyChanged(nameof(PauseResumeButtonText));
                }
            }
        }

        public BatteryCellModel? SelectedBattery
        {
            get => _selectedBattery;
            set
            {
                if (_selectedBattery != null)
                    _selectedBattery.IsSelected = false;

                if (SetProperty(ref _selectedBattery, value))
                {
                    if (_selectedBattery != null)
                        _selectedBattery.IsSelected = true;

                    UpdateChartForSelectedBattery();
                    OnPropertyChanged(nameof(SelectedBatteryName));
                }
            }
        }

        public string SelectedBatteryName => SelectedBattery?.Name ?? "No Selection";

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public TimeSpan StepTableElapsed
        {
            get => _stepTableElapsed;
            set
            {
                if (SetProperty(ref _stepTableElapsed, value))
                {
                    OnPropertyChanged(nameof(StepTableElapsedDisplay));
                }
            }
        }

        public string StepTableElapsedDisplay => $"Elapsed: {StepTableElapsed:hh\\:mm\\:ss}";

        public string PauseResumeButtonText => IsPaused ? "Resume" : "Pause/Resume";

        public bool CanStart => !IsRunning;
        public bool CanStop => IsRunning;
        public bool CanPauseResume => IsRunning;

        #endregion

        #region Stats

        public ExperimentStatsModel Stats { get; } = new();

        #endregion

        #region Commands

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand PauseResumeCommand { get; }
        public ICommand SelectBatteryCommand { get; }
        public ICommand FilterCommand { get; }

        #endregion

        public BatteryMonitorViewModel()
        {
            // Initialize commands
            StartCommand = new RelayCommand(Start, () => CanStart);
            StopCommand = new RelayCommand(Stop, () => CanStop);
            PauseResumeCommand = new RelayCommand(PauseResume, () => CanPauseResume);
            SelectBatteryCommand = new RelayCommand<BatteryCellModel>(SelectBattery);
            FilterCommand = new RelayCommand(ApplyFilter);

            // Initialize the view
            InitializeBatteryCells();
            InitializeMeasurementValues();
            InitializeStepTable();
            InitializeStats();
            InitializeChart();

            // Select first battery by default
            SelectedBattery = BatteryCells.FirstOrDefault();
        }

        #region Initialization Methods

        private void InitializeBatteryCells()
        {
            // Create 16 battery cells (2 rows x 8 columns as shown in screenshot)
            var batteryData = new[]
            {
                // Row 1
                new { Id = 1, Voltage = 15.0, Current = 5.0, Temp = 20.0, Status = EBatteryCellStatus.Normal },
                new { Id = 2, Voltage = 15.0, Current = 5.0, Temp = 20.0, Status = EBatteryCellStatus.Warning },
                new { Id = 3, Voltage = 12.0, Current = 5.0, Temp = 20.0, Status = EBatteryCellStatus.Normal },
                new { Id = 4, Voltage = 25.0, Current = 5.0, Temp = 20.0, Status = EBatteryCellStatus.Error },
                new { Id = 5, Voltage = 6.0, Current = 5.0, Temp = 20.0, Status = EBatteryCellStatus.Disabled },
                new { Id = 6, Voltage = 8.0, Current = 5.0, Temp = 20.0, Status = EBatteryCellStatus.Normal },
                new { Id = 7, Voltage = 9.0, Current = 5.0, Temp = 20.0, Status = EBatteryCellStatus.Normal },
                new { Id = 8, Voltage = 4.0, Current = 5.0, Temp = 20.0, Status = EBatteryCellStatus.Normal },
                // Row 2
                new { Id = 9, Voltage = 5.2, Current = 5.0, Temp = 20.0, Status = EBatteryCellStatus.Normal },
                new { Id = 10, Voltage = 4.2, Current = 5.0, Temp = 20.0, Status = EBatteryCellStatus.Normal },
                new { Id = 11, Voltage = 3.1, Current = 5.0, Temp = 20.0, Status = EBatteryCellStatus.Normal },
                new { Id = 12, Voltage = 6.0, Current = 5.0, Temp = 20.0, Status = EBatteryCellStatus.Normal },
                new { Id = 13, Voltage = 0.0, Current = 0.0, Temp = 0.0, Status = EBatteryCellStatus.Disabled },
                new { Id = 14, Voltage = 5.2, Current = 5.0, Temp = 20.0, Status = EBatteryCellStatus.Normal },
                new { Id = 15, Voltage = 0.0, Current = 0.0, Temp = 0.0, Status = EBatteryCellStatus.Disabled },
                new { Id = 16, Voltage = 9.5, Current = 5.0, Temp = 20.0, Status = EBatteryCellStatus.Normal },
            };

            foreach (var data in batteryData)
            {
                var cell = new BatteryCellModel
                {
                    Id = data.Id,
                    Name = $"Battery {data.Id}",
                    Voltage = data.Voltage,
                    Current = data.Current,
                    Temperature = data.Temp,
                    Status = data.Status,
                    IsDisabled = data.Status == EBatteryCellStatus.Disabled,
                    HasError = data.Status == EBatteryCellStatus.Error,
                    HasWarning = data.Status == EBatteryCellStatus.Warning,
                };

                // Initialize history data
                InitializeHistoryData(cell);

                BatteryCells.Add(cell);
            }
        }

        private void InitializeHistoryData(BatteryCellModel cell)
        {
            var random = new Random(cell.Id);
            
            // Generate sample voltage history (varying around the base voltage)
            for (int i = 0; i < 60; i++)
            {
                double baseVoltage = cell.IsDisabled ? 0 : cell.Voltage;
                double variation = random.NextDouble() * 0.5 - 0.25;
                cell.VoltageHistory.Add(Math.Max(0, baseVoltage * 0.8 + variation + (i * 0.02)));
            }

            // Generate sample current history
            for (int i = 0; i < 60; i++)
            {
                double baseCurrent = cell.IsDisabled ? 0 : cell.Current;
                double variation = random.NextDouble() * 0.3;
                cell.CurrentHistory.Add(Math.Max(0, baseCurrent * 0.5 + variation));
            }
        }

        private void InitializeMeasurementValues()
        {
            MeasurementValues.Add(new MeasurementValueModel { Label = "Voltage", Value = "5.6", Unit = "V" });
            MeasurementValues.Add(new MeasurementValueModel { Label = "Current", Value = "3.4", Unit = "A" });
            MeasurementValues.Add(new MeasurementValueModel { Label = "Temperature", Value = "5.6", Unit = "V" });
            MeasurementValues.Add(new MeasurementValueModel { Label = "Ampere Hour", Value = "5.6", Unit = "V" });
            MeasurementValues.Add(new MeasurementValueModel { Label = "Capacity", Value = "5.6", Unit = "V" });
            MeasurementValues.Add(new MeasurementValueModel { Label = "Voltage", Value = "5.6", Unit = "V" });
        }

        private void InitializeStepTable()
        {
            StepTableElapsed = TimeSpan.FromMinutes(15).Add(TimeSpan.FromSeconds(12));

            StepTableItems.Add(new StepTableItemModel
            {
                StepNumber = 1,
                Name = "Charge to 80%",
                Status = EStepStatus.Completed,
                TargetValue = 80,
                TargetUnit = "%"
            });

            StepTableItems.Add(new StepTableItemModel
            {
                StepNumber = 2,
                Name = "Wait for 10min",
                Status = EStepStatus.Running,
                IsActive = true,
                Duration = TimeSpan.FromMinutes(10)
            });

            StepTableItems.Add(new StepTableItemModel
            {
                StepNumber = 3,
                Name = "Discharge",
                Status = EStepStatus.Pending
            });

            StepTableItems.Add(new StepTableItemModel
            {
                StepNumber = 4,
                Name = "Wait for 10min",
                Status = EStepStatus.Pending,
                Duration = TimeSpan.FromMinutes(10)
            });
        }

        private void InitializeStats()
        {
            Stats.PatternName = "CC Charge";
            Stats.Elapsed = TimeSpan.FromMinutes(45).Add(TimeSpan.FromSeconds(12));
            Stats.ExperimentName = "Test battery cells";
            Stats.IsRunning = true;
        }

        private void InitializeChart()
        {
            // Generate time labels (0 to 60 seconds, stepping by 5)
            ChartLabels = Enumerable.Range(0, 13).Select(i => (i * 5).ToString()).ToArray();

            UpdateChartForSelectedBattery();
        }

        private void UpdateChartForSelectedBattery()
        {
            ChartSeries.Clear();

            if (SelectedBattery == null) return;

            // Voltage series (blue line)
            ChartSeries.Add(new LineSeries
            {
                Title = "Voltage (V)",
                Values = SelectedBattery.VoltageHistory,
                PointGeometry = null,
                Stroke = new SolidColorBrush(Color.FromRgb(59, 130, 246)), // Blue
                Fill = Brushes.Transparent,
                LineSmoothness = 0.5
            });

            // Current series (orange line)
            ChartSeries.Add(new LineSeries
            {
                Title = "Current (A)",
                Values = SelectedBattery.CurrentHistory,
                PointGeometry = null,
                Stroke = new SolidColorBrush(Color.FromRgb(249, 115, 22)), // Orange
                Fill = Brushes.Transparent,
                LineSmoothness = 0.5,
                ScalesYAt = 1 // Use secondary Y axis
            });

            OnPropertyChanged(nameof(ChartSeries));
        }

        #endregion

        #region Command Handlers

        private void Start()
        {
            if (IsRunning) return;

            IsRunning = true;
            IsPaused = false;
            _cts = new CancellationTokenSource();

            // Add log entry
            AddLog(ELogLevel.Info, $"Experiment \"{Stats.ExperimentName}\" started");
            AddLog(ELogLevel.Info, "Pattern \"CCCV Charge\" started");

            // Start update timer
            _updateTimer = new Timer(UpdateData, null, 0, 1000);
        }

        private void Stop()
        {
            if (!IsRunning) return;

            _cts?.Cancel();
            _updateTimer?.Dispose();
            _updateTimer = null;

            IsRunning = false;
            IsPaused = false;

            AddLog(ELogLevel.Info, "Experiment stopped");
        }

        private void PauseResume()
        {
            if (!IsRunning) return;

            IsPaused = !IsPaused;

            if (IsPaused)
            {
                AddLog(ELogLevel.Info, "Experiment paused");
            }
            else
            {
                AddLog(ELogLevel.Info, "Experiment resumed");
            }
        }

        private void SelectBattery(BatteryCellModel? battery)
        {
            if (battery != null && !battery.IsDisabled)
            {
                SelectedBattery = battery;
            }
        }

        private void ApplyFilter()
        {
            // Filter implementation - can be extended based on requirements
        }

        #endregion

        #region Helper Methods

        private void AddLog(ELogLevel level, string message)
        {
            var log = new SystemLogModel
            {
                Timestamp = DateTime.Now,
                Level = level,
                Message = message
            };

            // Add to beginning of collection (newest first)
            System.Windows.Application.Current?.Dispatcher.Invoke(() =>
            {
                SystemLogs.Insert(0, log);

                // Keep only last 100 logs
                while (SystemLogs.Count > 100)
                {
                    SystemLogs.RemoveAt(SystemLogs.Count - 1);
                }
            });
        }

        private void UpdateData(object? state)
        {
            if (IsPaused || !IsRunning) return;

            try
            {
                System.Windows.Application.Current?.Dispatcher.Invoke(() =>
                {
                    // Update elapsed times
                    StepTableElapsed = StepTableElapsed.Add(TimeSpan.FromSeconds(1));
                    Stats.Elapsed = Stats.Elapsed.Add(TimeSpan.FromSeconds(1));

                    // Simulate data updates for each battery
                    var random = new Random();
                    foreach (var battery in BatteryCells.Where(b => !b.IsDisabled))
                    {
                        // Add new data point
                        double voltageVariation = (random.NextDouble() - 0.5) * 0.1;
                        double currentVariation = (random.NextDouble() - 0.5) * 0.1;

                        if (battery.VoltageHistory.Count > 60)
                            battery.VoltageHistory.RemoveAt(0);
                        battery.VoltageHistory.Add(battery.Voltage + voltageVariation);

                        if (battery.CurrentHistory.Count > 60)
                            battery.CurrentHistory.RemoveAt(0);
                        battery.CurrentHistory.Add(battery.Current + currentVariation);
                    }

                    // Simulate occasional error
                    if (random.Next(0, 100) > 98)
                    {
                        AddLog(ELogLevel.Error, "Emergency stop because voltage > 5");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Update error: {ex.Message}");
            }
        }

        #endregion

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _updateTimer?.Dispose();
        }
    }
}

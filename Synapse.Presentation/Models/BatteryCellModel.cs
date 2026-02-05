using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts;
using Synapse.Shared.Enums;

namespace Synapse.Presentation.Models
{
    /// <summary>
    /// Represents a single battery cell in the monitoring grid
    /// </summary>
    public class BatteryCellModel : ObservableObject
    {
        private int _id;
        private string _name = string.Empty;
        private double _voltage;
        private double _current;
        private double _temperature;
        private bool _isSelected;
        private bool _isDisabled;
        private bool _hasError;
        private bool _hasWarning;
        private EBatteryCellStatus _status = EBatteryCellStatus.Normal;
        private ChartValues<double> _voltageHistory = new();
        private ChartValues<double> _currentHistory = new();

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// Voltage in Volts (e.g., 15V, 4.2V)
        /// </summary>
        public double Voltage
        {
            get => _voltage;
            set => SetProperty(ref _voltage, value);
        }

        /// <summary>
        /// Current in Amperes (e.g., 5A, 3.4A)
        /// </summary>
        public double Current
        {
            get => _current;
            set => SetProperty(ref _current, value);
        }

        /// <summary>
        /// Temperature in Celsius (e.g., 20oC)
        /// </summary>
        public double Temperature
        {
            get => _temperature;
            set => SetProperty(ref _temperature, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        /// <summary>
        /// Whether the battery cell is disabled/inactive (shown as gray)
        /// </summary>
        public bool IsDisabled
        {
            get => _isDisabled;
            set => SetProperty(ref _isDisabled, value);
        }

        /// <summary>
        /// Whether the battery has an error condition (shown with red border)
        /// </summary>
        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        /// <summary>
        /// Whether the battery has a warning condition (shown with yellow border)
        /// </summary>
        public bool HasWarning
        {
            get => _hasWarning;
            set => SetProperty(ref _hasWarning, value);
        }

        public EBatteryCellStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        /// <summary>
        /// Historical voltage data for charting
        /// </summary>
        public ChartValues<double> VoltageHistory
        {
            get => _voltageHistory;
            set => SetProperty(ref _voltageHistory, value);
        }

        /// <summary>
        /// Historical current data for charting
        /// </summary>
        public ChartValues<double> CurrentHistory
        {
            get => _currentHistory;
            set => SetProperty(ref _currentHistory, value);
        }

        /// <summary>
        /// Display string for voltage (e.g., "15V")
        /// </summary>
        public string VoltageDisplay => $"{Voltage:F1}V";

        /// <summary>
        /// Display string for current (e.g., "5A")
        /// </summary>
        public string CurrentDisplay => $"{Current:F1}A";

        /// <summary>
        /// Display string for temperature (e.g., "20oC")
        /// </summary>
        public string TemperatureDisplay => $"{Temperature:F0}oC";
    }
}

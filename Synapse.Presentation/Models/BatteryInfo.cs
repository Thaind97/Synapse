using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts;
using Synapse.Shared.Enums;

namespace Synapse.Presentation.Models
{
    public class BatteryInfo : ObservableObject
    {
        private string _name = string.Empty;
        private double _voltage;
        private double _current = 5.0;
        private double _temperature = 20.0;
        private int _stateOfCharge;
        private int _passCount;
        private bool _isSelected;
        private double _ampereHour;
        private double _capacity;
        private BatteryStatus _status = BatteryStatus.Normal;
        private ChartValues<double> _historyData = new();
        private ChartValues<double> _currentHistoryData = new();

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public double Voltage
        {
            get => _voltage;
            set => SetProperty(ref _voltage, value);
        }

        public double Current
        {
            get => _current;
            set => SetProperty(ref _current, value);
        }

        public double Temperature
        {
            get => _temperature;
            set => SetProperty(ref _temperature, value);
        }

        public int StateOfCharge
        {
            get => _stateOfCharge;
            set => SetProperty(ref _stateOfCharge, value);
        }

        public int PassCount
        {
            get => _passCount;
            set => SetProperty(ref _passCount, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public double AmpereHour
        {
            get => _ampereHour;
            set => SetProperty(ref _ampereHour, value);
        }

        public double Capacity
        {
            get => _capacity;
            set => SetProperty(ref _capacity, value);
        }

        public BatteryStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public ChartValues<double> HistoryData
        {
            get => _historyData;
            set => SetProperty(ref _historyData, value);
        }

        public ChartValues<double> CurrentHistoryData
        {
            get => _currentHistoryData;
            set => SetProperty(ref _currentHistoryData, value);
        }
    }
}

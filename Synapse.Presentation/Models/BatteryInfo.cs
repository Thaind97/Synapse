using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts;

namespace Synapse.Presentation.Models
{
    public class BatteryInfo : ObservableObject
    {
        private string _name = string.Empty;
        private double _voltage;
        private int _stateOfCharge;
        private int _passCount;
        private bool _isSelected;
        private ChartValues<double> _historyData = new();

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

        public ChartValues<double> HistoryData
        {
            get => _historyData;
            set => SetProperty(ref _historyData, value);
        }
    }
}

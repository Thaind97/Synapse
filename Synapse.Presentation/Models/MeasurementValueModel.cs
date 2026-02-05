using CommunityToolkit.Mvvm.ComponentModel;

namespace Synapse.Presentation.Models
{
    /// <summary>
    /// Represents a measurement value display item (Voltage, Current, Temperature, etc.)
    /// </summary>
    public class MeasurementValueModel : ObservableObject
    {
        private string _label = string.Empty;
        private string _value = string.Empty;
        private string _unit = string.Empty;

        public string Label
        {
            get => _label;
            set => SetProperty(ref _label, value);
        }

        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public string Unit
        {
            get => _unit;
            set => SetProperty(ref _unit, value);
        }

        /// <summary>
        /// Combined display string (e.g., "5.6 V")
        /// </summary>
        public string DisplayValue => $"{Value} {Unit}";
    }
}

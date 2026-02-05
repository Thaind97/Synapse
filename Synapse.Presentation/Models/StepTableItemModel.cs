using CommunityToolkit.Mvvm.ComponentModel;
using Synapse.Shared.Enums;

namespace Synapse.Presentation.Models
{
    /// <summary>
    /// Represents a step in the Step Table (Charge, Wait, Discharge, etc.)
    /// </summary>
    public class StepTableItemModel : ObservableObject
    {
        private int _stepNumber;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private EStepStatus _status = EStepStatus.Pending;
        private bool _isActive;
        private TimeSpan _duration;
        private double _targetValue;
        private string _targetUnit = string.Empty;

        public int StepNumber
        {
            get => _stepNumber;
            set => SetProperty(ref _stepNumber, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public EStepStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        /// <summary>
        /// Whether this step is currently active/running
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        public TimeSpan Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }

        public double TargetValue
        {
            get => _targetValue;
            set => SetProperty(ref _targetValue, value);
        }

        public string TargetUnit
        {
            get => _targetUnit;
            set => SetProperty(ref _targetUnit, value);
        }

        /// <summary>
        /// Display string for duration (e.g., "10min")
        /// </summary>
        public string DurationDisplay => Duration.TotalMinutes > 0 ? $"{Duration.TotalMinutes:F0}min" : string.Empty;
    }
}

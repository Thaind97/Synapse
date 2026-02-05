using CommunityToolkit.Mvvm.ComponentModel;

namespace Synapse.Presentation.Models
{
    /// <summary>
    /// Represents the stats panel data (Pattern name, elapsed time, etc.)
    /// </summary>
    public class ExperimentStatsModel : ObservableObject
    {
        private string _patternName = string.Empty;
        private TimeSpan _elapsed;
        private string _experimentName = string.Empty;
        private bool _isRunning;

        public string PatternName
        {
            get => _patternName;
            set => SetProperty(ref _patternName, value);
        }

        public TimeSpan Elapsed
        {
            get => _elapsed;
            set
            {
                if (SetProperty(ref _elapsed, value))
                {
                    OnPropertyChanged(nameof(ElapsedDisplay));
                }
            }
        }

        public string ExperimentName
        {
            get => _experimentName;
            set => SetProperty(ref _experimentName, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        /// <summary>
        /// Formatted elapsed time (e.g., "00:45:12")
        /// </summary>
        public string ElapsedDisplay => $"{Elapsed:hh\\:mm\\:ss}";
    }
}

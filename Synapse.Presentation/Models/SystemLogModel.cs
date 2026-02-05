using CommunityToolkit.Mvvm.ComponentModel;
using Synapse.Shared.Enums;

namespace Synapse.Presentation.Models
{
    /// <summary>
    /// Represents a system log entry
    /// </summary>
    public class SystemLogModel : ObservableObject
    {
        private DateTime _timestamp;
        private ELogLevel _level = ELogLevel.Info;
        private string _message = string.Empty;
        private string _source = string.Empty;

        public DateTime Timestamp
        {
            get => _timestamp;
            set => SetProperty(ref _timestamp, value);
        }

        public ELogLevel Level
        {
            get => _level;
            set => SetProperty(ref _level, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public string Source
        {
            get => _source;
            set => SetProperty(ref _source, value);
        }

        /// <summary>
        /// Formatted timestamp string (e.g., "[14:47:05]")
        /// </summary>
        public string TimestampDisplay => $"[{Timestamp:HH:mm:ss}]";

        /// <summary>
        /// Full display string with level prefix
        /// </summary>
        public string FullDisplay => $"{TimestampDisplay} {Level.ToString().ToUpper()}: {Message}";
    }
}

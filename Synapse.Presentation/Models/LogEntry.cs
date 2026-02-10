using CommunityToolkit.Mvvm.ComponentModel;
using Synapse.Shared.Enums;

namespace Synapse.Presentation.Models
{
    public class LogEntry : ObservableObject
    {
        private string _timestamp = string.Empty;
        private string _message = string.Empty;
        private LogLevel _level = LogLevel.Info;

        public string Timestamp
        {
            get => _timestamp;
            set => SetProperty(ref _timestamp, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public LogLevel Level
        {
            get => _level;
            set => SetProperty(ref _level, value);
        }
    }
}

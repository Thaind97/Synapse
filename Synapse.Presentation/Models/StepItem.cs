using CommunityToolkit.Mvvm.ComponentModel;
using Synapse.Shared.Enums;

namespace Synapse.Presentation.Models
{
    public class StepItem : ObservableObject
    {
        private string _step = string.Empty;
        private string _description = string.Empty;
        private StepStatus _status = StepStatus.Pending;

        public string Step
        {
            get => _step;
            set => SetProperty(ref _step, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public StepStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
    }
}

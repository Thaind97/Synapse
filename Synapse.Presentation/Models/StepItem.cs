using CommunityToolkit.Mvvm.ComponentModel;

namespace Synapse.Presentation.Models
{
    public class StepItem : ObservableObject
    {
        private string _step = string.Empty;
        private string _description = string.Empty;

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
    }
}

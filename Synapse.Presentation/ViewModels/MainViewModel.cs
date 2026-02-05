using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Synapse.Services;
using Synapse.Presentation.Services;

namespace Synapse.Presentation.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly IDashboardService _dashboardService;

        // Expose Localization for XAML binding
        public ILocalizationService Localization { get; }

        private object? _currentView;
        public object? CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        private string _selectedMenuKey = "Dashboard";
        public string SelectedMenuKey
        {
            get => _selectedMenuKey;
            set => SetProperty(ref _selectedMenuKey, value);
        }

        private string _currentPageKey = "Dashboard";
        public string CurrentPageTitle => Localization[_currentPageKey];

        public ICommand NavigateCommand { get; }
        public ICommand LogoutCommand { get; }

        /// <summary>
        /// Callback to be invoked when user requests logout
        /// </summary>
        public Action? OnLogoutRequested { get; set; }

        public MainViewModel(IDashboardService dashboardService, ILocalizationService localization)
        {
            _dashboardService = dashboardService;
            Localization = localization;

            NavigateCommand = new RelayCommand<string>(Navigate);
            LogoutCommand = new RelayCommand(Logout);

            // Initialize with Dashboard ViewModel
            Navigate("Dashboard");
        }

        private void Navigate(string? viewName)
        {
            if (string.IsNullOrEmpty(viewName)) return;

            switch (viewName)
            {
                case "Dashboard":
                    // Assign ViewModel; DataTemplate in App.xaml will render DashboardView
                    CurrentView = new DashboardViewModel(_dashboardService);
                    SetCurrentPage("Dashboard");
                    break;
                case "EVControl":
                    // Assign ViewModel; DataTemplate in App.xaml will render EVControlView
                    CurrentView = new EVControlViewModel();
                    SetCurrentPage("EVControl");
                    break;
                case "SequenceEditor":
                    var sequenceVm = Synapse.Shared.Helper.ServiceHelper.GetRequiredService<SequenceEditorViewModel>();
                    CurrentView = sequenceVm;
                    _ = sequenceVm.LoadSequencesCommand.ExecuteAsync(null);
                    SetCurrentPage("SiteManagement");
                    break;
                case "ManageBatteries":
                    var mbVm = Synapse.Shared.Helper.ServiceHelper.GetRequiredService<ManageBatteriesViewModel>();
                    CurrentView = mbVm;
                    // call LoadDataAsync if available
                    var loadMethod = mbVm.GetType().GetMethod("LoadDataAsync", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                    if (loadMethod != null)
                    {
                        var result = loadMethod.Invoke(mbVm, null);
                        if (result is System.Threading.Tasks.Task t) _ = t;
                    }
                    SetCurrentPage("ManageBatteries");
                    break;
                default:
                    break;
            }
        }

        private void SetCurrentPage(string key)
        {
            SelectedMenuKey = key;
            _currentPageKey = key;
            OnPropertyChanged(nameof(CurrentPageTitle));
        }

        private void Logout()
        {
            OnLogoutRequested?.Invoke();
        }

        // Public helper to be callable from code-behind
        public void NavigateTo(string viewName) => Navigate(viewName);
    }
}

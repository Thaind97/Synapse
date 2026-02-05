using Microsoft.Extensions.Options;
using Synapse.Presentation.Services;
using Synapse.Services.Services.Abstraction;
using Synapse.Shared.Options;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Synapse.Presentation.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authService;
        private readonly AppSettings _appSettings;

        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isLoggingIn;
        private string _selectedLanguage = string.Empty;

        // Expose Localization for XAML binding
        public ILocalizationService Localization { get; }

        public ObservableCollection<string> Languages { get; } =
            new() { "English", "“ú–{Œê" };

        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                _selectedLanguage = value;
                OnPropertyChanged();
                var cultureCode = LocalizationService.GetCultureCode(value);
                Localization.SetLanguage(cultureCode);
            }
        }

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public bool IsLoggingIn
        {
            get => _isLoggingIn;
            set
            {
                _isLoggingIn = value;
                OnPropertyChanged();
                ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Current login mode from settings
        /// </summary>
        public string CurrentLoginMode => _appSettings.LoginMode;

        public ICommand LoginCommand { get; }

        public Action? OnLoginSuccess { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public LoginViewModel(
            IUserService userService,
            IAuthenticationService authService,
            ILocalizationService localization,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _authService = authService;
            Localization = localization;
            _appSettings = appSettings.Value;

            LoginCommand = new RelayCommand(
                async _ => await LoginAsync(),
                _ => !IsLoggingIn
            );

            SelectedLanguage = Languages[0]; // Default English
        }

        private async Task LoginAsync()
        {
            IsLoggingIn = true;
            ErrorMessage = string.Empty;

            try
            {
                // Switch case based on LoginMode from appsettings.json
                switch (_appSettings.LoginMode.ToUpperInvariant())
                {
                    case "API":
                        await LoginWithApiAsync();
                        break;

                    case "SQLITE":
                    default:
                        await LoginWithSqliteAsync();
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsLoggingIn = false;
            }
        }

        /// <summary>
        /// Login v?i API b?n th? 3 - l?y access token v? refresh token
        /// </summary>
        private async Task LoginWithApiAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = Localization["InvalidCredentials"];
                return;
            }

            var result = await _authService.LoginAsync(Username, Password);

            if (result.Success)
            {
                // Access token: _authService.GetAccessToken()
                // Refresh token: _authService.GetRefreshToken()
                // User info: result.User
                OnLoginSuccess?.Invoke();
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? Localization["InvalidCredentials"];
            }
        }

        /// <summary>
        /// Login v?i SQLite local database
        /// </summary>
        private async Task LoginWithSqliteAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = Localization["InvalidCredentials"];
                return;
            }

            var user = await _userService.AuthenticateAsync(Username, Password);

            if (user != null)
            {
                OnLoginSuccess?.Invoke();
            }
            else
            {
                ErrorMessage = Localization["InvalidCredentials"];
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public class RelayCommand : ICommand
        {
            private readonly Predicate<object?>? _canExecute;
            private readonly Func<object?, Task> _executeAsync;

            public RelayCommand(Func<object?, Task> execute, Predicate<object?>? canExecute = null)
            {
                _executeAsync = execute;
                _canExecute = canExecute;
            }

            public bool CanExecute(object? parameter) =>
                _canExecute?.Invoke(parameter) ?? true;

            public async void Execute(object? parameter) =>
                await _executeAsync(parameter);

            public event EventHandler? CanExecuteChanged;

            public void RaiseCanExecuteChanged() =>
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

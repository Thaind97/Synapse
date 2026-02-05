using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Synapse.Infrastructure;
using Synapse.Presentation.Configuration;
using Synapse.Presentation.ViewModels;
using Synapse.Presentation.Views;
using Synapse.Services;
using Synapse.Services.Services.Abstraction;
using System.IO;
using System.Windows;

namespace Synapse.Presentation
{
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;
        private IConfiguration? _configuration;
        private static readonly string LogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "startup.log");

        private void Log(string message)
        {
            try
            {
                File.AppendAllText(LogFile, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {message}\n");
            }
            catch { }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Add global exception handlers
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                var ex = args.ExceptionObject as Exception;
                Log($"FATAL Unhandled Exception: {ex?.Message}\n{ex?.StackTrace}");
                MessageBox.Show($"Unhandled Exception:\n{ex?.Message}\n\n{ex?.StackTrace}", "Fatal Error");
            };

            DispatcherUnhandledException += (s, args) =>
            {
                Log($"Dispatcher Exception: {args.Exception.Message}\n{args.Exception.StackTrace}");
                MessageBox.Show($"Dispatcher Exception:\n{args.Exception.Message}\n\n{args.Exception.StackTrace}", "Error");
                args.Handled = true;
            };

            try
            {
                // Load configuration
                var configuration = AppSettingsLoader.LoadConfiguration();

                // Configure services
                var services = new ServiceCollection();

                // Add Logging
                services.AddLogging(builder => builder.AddDebug());

                // Add Configuration with IOptions pattern
                services.AddSingleton(configuration);
                services.AddAppSettings(configuration);

                // Add layers - each layer has its own ServiceRegistration
                services.AddInfrastructureServices();           // Infrastructure layer (Database)
                services.AddSynapseServices(configuration);     // Services layer (HTTP, Auth, Business)
                services.AddPresentationServices();             // Presentation layer (ViewModels, Localization)

                _serviceProvider = services.BuildServiceProvider();

                // Initialize global ServiceHelper
                Synapse.Shared.Helper.ServiceHelper.ServiceProvider = _serviceProvider;

                this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                ShowLoginAndMain();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Startup Error:\n{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void ShowLoginAndMain()
        {
            if (_serviceProvider == null) return;

            try
            {
            //    var loginViewModel = _serviceProvider.GetRequiredService<LoginViewModel>();
            //    var loginView = new LoginView(loginViewModel);

            //    bool? loginResult = loginView.ShowDialog();
            //    Log($"Login result: {loginResult}");

            //    if (loginResult != true)
            //    {
            //        Shutdown();
            //        return;
            //    }

            //    foreach (var window in Application.Current.Windows.OfType<MainWindow>().ToList())
            //    {
            //        window.Close();
            //    }

                var mainVm = _serviceProvider.GetRequiredService<MainViewModel>();

                mainVm.OnLogoutRequested = () =>
                {
                    Task.Run(async () =>
                    {
                        var authService = _serviceProvider.GetRequiredService<IAuthenticationService>();
                        await authService.LogoutAsync();
                    });

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (this.MainWindow is MainWindow mainWindow)
                        {
                            mainWindow.Close();
                        }
                        ShowLoginAndMain();
                    });
                };

                var mainWindow = new MainWindow { DataContext = mainVm };
                this.MainWindow = mainWindow;
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error:\n{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}

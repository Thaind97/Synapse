using System.Windows;
using Synapse.Presentation.ViewModels;

namespace Synapse.Presentation.Views
{
    public partial class LoginView : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginView(LoginViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;

            // Bind PasswordBox.Password to ViewModel.Password
            PasswordBox.PasswordChanged += (s, e) =>
            {
                _viewModel.Password = PasswordBox.Password;
            };

            // When login succeeds, close the dialog with DialogResult=true
            _viewModel.OnLoginSuccess = () =>
            {
                DialogResult = true;
                Close();
            };
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Registration is not implemented yet.", "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

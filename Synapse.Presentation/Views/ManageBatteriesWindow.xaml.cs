using System.Windows;
using Synapse.Presentation.ViewModels;

namespace Synapse.Presentation.Views
{
    public partial class ManageBatteriesWindow : Window
    {
        public ManageBatteriesWindow()
        {
            InitializeComponent();
        }

        private async void ManageBatteriesWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ManageBatteriesViewModel vm)
            {
                await vm.LoadDataAsync();
            }
        }
    }
}

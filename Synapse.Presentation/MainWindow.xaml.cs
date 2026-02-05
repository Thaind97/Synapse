using System.Windows;
using Synapse.Presentation.ViewModels;

namespace Synapse.Presentation;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void BtnDashboard_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.NavigateTo("Dashboard");
        }
    }

    private void BtnEVControl_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.NavigateTo("EVControl");
        }
    }

    private void BtnSiteManagement_Click(object sender, RoutedEventArgs e)
    {
         if (DataContext is MainViewModel vm)
        {
            vm.NavigateTo("SequenceEditor");
        }
    }

    private void BtnBatteryMonitor_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.NavigateTo("BatteryMonitor");
        }
    }
}
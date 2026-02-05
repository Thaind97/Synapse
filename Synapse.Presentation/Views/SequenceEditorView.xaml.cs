using System.Windows;
using System.Windows.Controls;
using Synapse.Presentation.Views;

namespace Synapse.Presentation.Views
{
    public partial class SequenceEditorView : UserControl
    {
        public SequenceEditorView()
        {
            InitializeComponent();
        }

        private void ManageBatteries_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as Synapse.Presentation.ViewModels.SequenceEditorViewModel;
            var wnd = new ManageBatteriesWindow();
            wnd.DataContext = new Synapse.Presentation.ViewModels.ManageBatteriesViewModel(Synapse.Shared.Helper.ServiceHelper.GetRequiredService<Synapse.Services.Services.Abstraction.ISequenceService>());
            wnd.Owner = Application.Current.MainWindow;
            wnd.ShowDialog();
        }
    }
}

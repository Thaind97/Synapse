using System.Windows.Controls;
using System.Windows.Input;

namespace Synapse.Presentation.Views
{
    public partial class EVControlView : UserControl
    {
        public EVControlView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles mouse wheel scrolling horizontally for the channel panel
        /// </summary>
        private void ChannelScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                // Scroll horizontally instead of vertically
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - e.Delta);
                e.Handled = true;
            }
        }
    }
}

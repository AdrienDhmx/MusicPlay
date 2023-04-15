using GongSolutions.Wpf.DragDrop.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicPlayUI.MVVM.Views
{
    /// <summary>
    /// Logique d'interaction pour QueueView.xaml
    /// </summary>
    public partial class QueueView : UserControl
    {
        public QueueView()
        {
            InitializeComponent();
        }

        private void VirtualizingGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void QueueTracks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Task.Delay(300);
            double height = QueueTracks.ActualHeight;
            int totalTracks = QueueTracks.Items.Count;
            double itemHeight = height / (double)totalTracks;
            double verticalOffset = itemHeight * ((double)QueueTracks.SelectedIndex - 2);
            ScrollViewer QueueScroll = QueueTracks.GetVisualDescendent<ScrollViewer>();

            if (verticalOffset is not double.NaN && QueueScroll is not null)
            {
                QueueScroll.ScrollToVerticalOffset(verticalOffset);
            }
        }
    }
}

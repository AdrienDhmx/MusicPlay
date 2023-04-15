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

namespace MusicPlayUI.MVVM.Views.PopupViews
{
    /// <summary>
    /// Interaction logic for QueueDrawerView.xaml
    /// </summary>
    public partial class QueueDrawerView : UserControl
    {
        public QueueDrawerView()
        {
            InitializeComponent();
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

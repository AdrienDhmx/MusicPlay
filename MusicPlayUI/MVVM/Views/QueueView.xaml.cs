using DynamicScrollViewer;
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

        private async void QueueTracks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Task.Delay(300);
            double height = QueueTracks.ActualHeight;
            int totalTracks = QueueTracks.Items.Count;
            double itemHeight = height / (double)totalTracks;
            double verticalOffset = itemHeight * ((double)QueueTracks.SelectedIndex - 2);
            DynamicScrollViewer.DynamicScrollViewer QueueScroll = QueueTracks.GetVisualDescendent<DynamicScrollViewer.DynamicScrollViewer>();

            if (verticalOffset is not double.NaN && QueueScroll is not null)
            {
                QueueScroll.ScrollToVerticalOffsetWithAnimation(verticalOffset);
            }
        }

        //private void QueueTracks_Loaded(object sender, RoutedEventArgs e)
        //{
        //    DynamicScrollViewer.DynamicScrollViewer QueueScroll = QueueTracks.GetVisualDescendent<DynamicScrollViewer.DynamicScrollViewer>();

        //    if(QueueScroll is not null)
        //    {
        //        QueueScroll.ScrollToItem(QueueTracks.SelectedItem);
        //        QueueScroll.UpdateIsInViewPort();
        //    } 
        //}
    }
}

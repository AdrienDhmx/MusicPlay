using GongSolutions.Wpf.DragDrop.Utilities;
using MusicPlayUI.Controls;
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

            if(QueueTracks.Items.Count < QueueTracks.SelectedIndex && QueueTracks.Items.GetItemAt(QueueTracks.SelectedIndex) is UIElement element)
            {
                AsyncImage image = element.GetVisualDescendent<AsyncImage>();
                if(image is not null && image.IsInViewport)
                {
                    return;
                }
            }
            
            if (QueueTracks.Template.FindName("PART_ContentHost", QueueTracks) is DynamicScrollViewer.DynamicScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToItem(QueueTracks.SelectedItem);
            }
        }

        private void QueueTracks_Loaded(object sender, RoutedEventArgs e)
        {
            if (QueueTracks.Template.FindName("PART_ContentHost", QueueTracks) is DynamicScrollViewer.DynamicScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToItem(QueueTracks.SelectedItem);
            }
        }


    }
}

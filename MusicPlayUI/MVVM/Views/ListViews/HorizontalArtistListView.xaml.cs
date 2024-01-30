using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using MusicPlay.Database.Models;
using MusicPlayUI.MVVM.Views.Windows;

namespace MusicPlayUI.MVVM.Views.ListViews
{
    /// <summary>
    /// Interaction logic for HorizontalArtistListView.xaml
    /// </summary>
    public partial class HorizontalArtistListView : UserControl
    {
        public HorizontalArtistListView()
        {
            InitializeComponent();
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            DynamicScrollViewer.DynamicScrollViewer parentScrollviewer = MainWindow.FindParent<DynamicScrollViewer.DynamicScrollViewer>(e.Source as DependencyObject);
            if (parentScrollviewer != null)
            {
                parentScrollviewer.ScrollToVerticalOffset(parentScrollviewer.VerticalOffset - e.Delta / 3);
                e.Handled = true;
            }
        }

        public ObservableCollection<Artist> Artists
        {
            get { return (ObservableCollection<Artist>)GetValue(ArtistsProperty); }
            set { SetValue(ArtistsProperty, value); }
        }

        public static readonly DependencyProperty ArtistsProperty =
            DependencyProperty.Register("Artists", typeof(ObservableCollection<Artist>), typeof(HorizontalArtistListView), new PropertyMetadata(new ObservableCollection<Artist>()));


    }
}

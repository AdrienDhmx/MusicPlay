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
using MusicPlayModels.MusicModels;
using MusicPlayUI.MVVM.Models;

namespace MusicPlayUI.MVVM.Views.ListViews
{
    /// <summary>
    /// Interaction logic for TrackListView.xaml
    /// </summary>
    public partial class TrackListView : UserControl
    {
        public TrackListView()
        {
            InitializeComponent();
        }

        public ObservableCollection<UIOrderedTrackModel> TrackModels
        {
            get { return (ObservableCollection<UIOrderedTrackModel>)GetValue(TrackModelsProperty); }
            set { SetValue(TrackModelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllTracks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrackModelsProperty =
            DependencyProperty.Register("TrackModels", typeof(ObservableCollection<UIOrderedTrackModel>), typeof(TrackListView), new PropertyMetadata(new ObservableCollection<UIOrderedTrackModel>()));


        public Visibility AlbumVisibility
        {
            get { return (Visibility)GetValue(AlbumVisibilityProperty); }
            set { SetValue(AlbumVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AlbumVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AlbumVisibilityProperty =
            DependencyProperty.Register("AlbumVisibility", typeof(Visibility), typeof(TrackListView), new PropertyMetadata(Visibility.Visible));


        public Visibility CoverVisibility
        {
            get { return (Visibility)GetValue(CoverVisibilityProperty); }
            set { SetValue(CoverVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CoverVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CoverVisibilityProperty =
            DependencyProperty.Register("CoverVisibility", typeof(Visibility), typeof(TrackListView), new PropertyMetadata(Visibility.Visible));


    }
}

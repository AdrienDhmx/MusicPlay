using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MusicPlay.Database.Models.DataBaseModels;

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

        public ObservableCollection<OrderedTrack> TrackModels
        {
            get { return (ObservableCollection<OrderedTrack>)GetValue(TrackModelsProperty); }
            set { SetValue(TrackModelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllTracks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrackModelsProperty =
            DependencyProperty.Register("TrackModels", typeof(ObservableCollection<OrderedTrack>), typeof(TrackListView), new PropertyMetadata(new ObservableCollection<OrderedTrack>()));


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

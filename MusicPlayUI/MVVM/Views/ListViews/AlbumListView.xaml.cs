using GongSolutions.Wpf.DragDrop.Utilities;
using MusicPlay.Database.Models;
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

namespace MusicPlayUI.MVVM.Views.ListViews
{
    /// <summary>
    /// Interaction logic for AlbumListView.xaml
    /// </summary>
    public partial class AlbumListView : UserControl
    {
        public AlbumListView()
        {
            InitializeComponent();
        }

        public ObservableCollection<Album> Albums
        {
            get { return (ObservableCollection<Album>)GetValue(AlbumsProperty); }
            set { SetValue(AlbumsProperty, value); }
        }

        public static readonly DependencyProperty AlbumsProperty =
            DependencyProperty.Register("Albums", typeof(ObservableCollection<Album>), typeof(AlbumListView), new PropertyMetadata(new ObservableCollection<Album>()));




        public bool ShowArtist
        {
            get { return (bool)GetValue(ShowArtistProperty); }
            set { SetValue(ShowArtistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for showArtist.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowArtistProperty =
            DependencyProperty.Register("ShowArtist", typeof(bool), typeof(AlbumListView), new PropertyMetadata(true));


    }
}

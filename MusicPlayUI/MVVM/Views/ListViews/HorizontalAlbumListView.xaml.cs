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
    /// Interaction logic for HorizontalAlbumListView.xaml
    /// </summary>
    public partial class HorizontalAlbumListView : UserControl
    {
        public HorizontalAlbumListView()
        {
            InitializeComponent();
        }

        public ObservableCollection<Album> Albums
        {
            get { return (ObservableCollection<Album>)GetValue(AlbumsProperty); }
            set { SetValue(AlbumsProperty, value); }
        }

        public static readonly DependencyProperty AlbumsProperty =
            DependencyProperty.Register("Albums", typeof(ObservableCollection<Album>), typeof(HorizontalAlbumListView), new PropertyMetadata(new ObservableCollection<Album>()));
    }
}

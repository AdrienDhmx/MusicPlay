using GongSolutions.Wpf.DragDrop.Utilities;
using MusicPlayModels.MusicModels;
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

        public ObservableCollection<AlbumModel> Albums
        {
            get { return (ObservableCollection<AlbumModel>)GetValue(AlbumsProperty); }
            set { SetValue(AlbumsProperty, value); }
        }

        public static readonly DependencyProperty AlbumsProperty =
            DependencyProperty.Register("Albums", typeof(ObservableCollection<AlbumModel>), typeof(AlbumListView), new PropertyMetadata(new ObservableCollection<AlbumModel>()));

    }
}

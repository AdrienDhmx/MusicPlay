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

        public ObservableCollection<Artist> Artists
        {
            get { return (ObservableCollection<Artist>)GetValue(ArtistsProperty); }
            set { SetValue(ArtistsProperty, value); }
        }

        public static readonly DependencyProperty ArtistsProperty =
            DependencyProperty.Register("Artists", typeof(ObservableCollection<Artist>), typeof(HorizontalArtistListView), new PropertyMetadata(new ObservableCollection<Artist>()));


    }
}

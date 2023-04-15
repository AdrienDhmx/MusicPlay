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

        public ObservableCollection<ArtistModel> Artists
        {
            get { return (ObservableCollection<ArtistModel>)GetValue(ArtistsProperty); }
            set { SetValue(ArtistsProperty, value); }
        }

        public static readonly DependencyProperty ArtistsProperty =
            DependencyProperty.Register("Artists", typeof(ObservableCollection<ArtistModel>), typeof(HorizontalArtistListView), new PropertyMetadata(new ObservableCollection<ArtistModel>()));


    }
}

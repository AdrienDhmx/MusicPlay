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
    /// Interaction logic for ArtistListView.xaml
    /// </summary>
    public partial class ArtistListView : UserControl
    {
        public ArtistListView()
        {
            InitializeComponent();
        }

        public ObservableCollection<ArtistModel> Artists
        {
            get { return (ObservableCollection<ArtistModel>)GetValue(ArtistsProperty); }
            set { SetValue(ArtistsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Artists.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArtistsProperty =
            DependencyProperty.Register("Artists", typeof(ObservableCollection<ArtistModel>), typeof(ArtistListView), new PropertyMetadata(new ObservableCollection<ArtistModel>()));

    }

}

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
using MusicPlayModels.MusicModels;

namespace MusicPlayUI.MVVM.Views.ListViews
{
    /// <summary>
    /// Interaction logic for ArtistsRelationListView.xaml
    /// </summary>
    public partial class ArtistsRelationListView : UserControl
    {
        public ArtistsRelationListView()
        {
            InitializeComponent();
        }

        public List<ArtistDataRelation> Artists
        {
            get { return (List<ArtistDataRelation>)GetValue(ArtistsProperty); }
            set { SetValue(ArtistsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Artists.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArtistsProperty =
            DependencyProperty.Register("Artists", typeof(List<ArtistDataRelation>), typeof(ArtistsRelationListView), new PropertyMetadata(new List<ArtistDataRelation>()));


        public int AcceptAlbumArtist
        {
            get { return (int)GetValue(AcceptAlbumArtistProperty); }
            set { SetValue(AcceptAlbumArtistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AcceptAlbumArtist.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AcceptAlbumArtistProperty =
            DependencyProperty.Register("AcceptAlbumArtist", typeof(int), typeof(ArtistsRelationListView), new PropertyMetadata(-1));



        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ArtistsRelationListView), new PropertyMetadata(defaultValue: null));


    }
}
